﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WishAPic.Data;
using WishAPic.DTO;
using WishAPic.Identity;
using WishAPic.Models;
using Microsoft.AspNetCore.Authorization;
using WishAPic.Services;
using WishAPic.ServiceContracts;

namespace WishAPic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly IImagesAdderService _imagesAdderService;
        private readonly IImagesGetterService _imagesGetterService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImagesDeleterService _imagesDeleterService;
        public ImagesController(ILogger<ImagesController> logger, IImagesAdderService imagesAdderService,
            IImagesGetterService imagesGetterService, IImagesDeleterService imagesDeleterService, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _imagesAdderService = imagesAdderService;
            _imagesGetterService = imagesGetterService;
            _imagesDeleterService = imagesDeleterService;
            _userManager = userManager;
        }

        [HttpPost("AddToFavorites")]
        public async Task<ActionResult<ImageData>> AddToFavoritesAsync([FromBody] ImageDataDTO imageDataDto)
        {
            if (imageDataDto == null)
                return Problem("Image Data is Null");

            var imageData = await _imagesGetterService.FindByIdAsync(imageDataDto.Id);

            if (imageData == null)
                return NotFound("Image Not Found");

            //ImageData image = new ImageData
            //{
            //    ImageId = imageDataDto.Id,
            //    UserId = imageDataDto.UserId,
            //    Prompt = imageDataDto.Prompt,
            //    IsFavorite = true,
            //    Image = Convert.FromBase64String(imageDataDto.Image
            //        .Substring(imageDataDto.Image.IndexOf(",") + 1))
            //};
            _imagesAdderService.AddToFavorites(imageData);

            return Ok(imageData);
        }
        [HttpGet("GetFavorites")]
        public async Task<IActionResult> GetFavoritesAsync(Guid userId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (user == null)
                return NotFound("User Not Found");

            var images = await _imagesGetterService.GetFavorites(userId);
            if (images.Count() == 0)
                return NotFound("No Items in Favorites");

            var favoriteImages = images.Select(imgData => new
            {
                imgData.ImageId,
                imgData.UserId,
                imgData.Prompt,
                Image = File(imgData.Image, "image/png")
            });

            return Ok(favoriteImages);
        }
        [HttpDelete("DeleteFromFavorites")]
        public async Task<IActionResult> DeleteFromFavoritesAsync(ImageDataDTO imageDataDTO)
        {
            var imageData = await _imagesGetterService.FindByIdAsync(imageDataDTO.Id);
            
            if (imageData == null)
                return NotFound("Image Not Found");
            
                
            
            //ImageData imageData = new ImageData
            //{
            //    ImageId = imageDataDTO.Id,
            //    UserId = imageDataDTO.UserId,
            //    Prompt = imageDataDTO.Prompt,
            //    Image = Convert.FromBase64String(imageDataDTO.Image
            //        .Substring(imageDataDTO.Image.IndexOf(",") + 1))
            //};
            ImageData imgData = await _imagesDeleterService.DeleteFromFavorites(imageData);
            return Ok(imgData);
        }

        [HttpDelete("DeleteFromHistory")]
        public async Task<IActionResult> DeleteFromHistory(ImageDataDTO imageDataDTO)
        {
            var imageData = await _imagesGetterService.FindByIdAsync(imageDataDTO.Id);

            if (imageData == null)
                return NotFound("Image Not Found");

            //ImageData imageData = new ImageData
            //{
            //    ImageId = imageDataDTO.Id,
            //    UserId = imageDataDTO.UserId,
            //    Prompt = imageDataDTO.Prompt,
            //    Image = Convert.FromBase64String(imageDataDTO.Image
            //        .Substring(imageDataDTO.Image.IndexOf(",") + 1))
            //};
            ImageData imgData = _imagesDeleterService.DeleteFromHistory(imageData);
            return Ok(imgData);
        }

    }
}

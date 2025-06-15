using Microsoft.AspNetCore.Http;
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
        private readonly IImagesDeleterService _imagesDeleterService;
        public ImagesController(ILogger<ImagesController> logger, IImagesAdderService imagesAdderService,
            IImagesGetterService imagesGetterService, IImagesDeleterService imagesDeleterService)
        {
            _logger = logger;
            _imagesAdderService = imagesAdderService;
            _imagesGetterService = imagesGetterService;
            _imagesDeleterService = imagesDeleterService;
        }

        [HttpPost("AddToFavorites")]
        public ActionResult<ImageData> AddToFavorites([FromBody] ImageDataDTO imageData)
        {
            if (imageData == null)
                return Problem("Image Data is Null");
            _logger.LogError("AddToFavorites");


            ImageData image = new ImageData
            {
                ImageId = imageData.Id,
                UserId = imageData.UserId,
                Prompt = imageData.Prompt,
                IsFavorite = true,
                Image = Convert.FromBase64String(imageData.Image
                    .Substring(imageData.Image.IndexOf(",") + 1))
            };
            _imagesAdderService.AddToFavorites(image);
            _logger.LogError(image.ToString());

            return Ok();
        }
        [HttpGet("GetFavorites")]
        public async Task<IActionResult> GetFavoritesAsync(Guid userId)
        {

            var images = (await _imagesGetterService.GetFavorites(userId)).Select(imgData => new
            {
                imgData.ImageId,
                imgData.UserId,
                imgData.Prompt,
                Image = File(imgData.Image, "image/png")
            });

            return Ok(images);
        }
        [HttpDelete("DeleteFromFavorites")]
        public async Task<IActionResult> DeleteFromFavoritesAsync(ImageDataDTO imageDataDTO)
        {
            ImageData imageData = new ImageData
            {
                ImageId = imageDataDTO.Id,
                UserId = imageDataDTO.UserId,
                Prompt = imageDataDTO.Prompt,
                Image = Convert.FromBase64String(imageDataDTO.Image
                    .Substring(imageDataDTO.Image.IndexOf(",") + 1))
            };
            ImageData imgData = await _imagesDeleterService.DeleteFromFavorites(imageData);
            return Ok(imgData);
        }

        [HttpDelete("DeleteFromHistory")]
        public IActionResult DeleteFromHistory(ImageDataDTO imageDataDTO)
        {
            ImageData imageData = new ImageData
            {
                ImageId = imageDataDTO.Id,
                UserId = imageDataDTO.UserId,
                Prompt = imageDataDTO.Prompt,
                Image = Convert.FromBase64String(imageDataDTO.Image
                    .Substring(imageDataDTO.Image.IndexOf(",") + 1))
            };
            ImageData imgData = _imagesDeleterService.DeleteFromHistory(imageData);
            return Ok(imgData);
        }

    }
}

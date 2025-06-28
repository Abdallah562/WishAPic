using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WishAPic.Identity;
using WishAPic.Models;
using WishAPic.ServiceContracts;
using WishAPic.Services;

namespace WishAPic.Controllers
{
    [ApiController]
    [Route("api/sdxl")]
    public class SDXLController : ControllerBase
    {
        private readonly IImageGenerator _imageGenerator;
        private readonly IImagesGetterService _imagesGetterService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SDXLController> _logger;

        public SDXLController(ILogger<SDXLController> logger, IImageGenerator imageGenerator, IImagesGetterService imagesGetterService, UserManager<ApplicationUser> userManager)
        {
            _imageGenerator = imageGenerator;
            _logger = logger;
            _imagesGetterService = imagesGetterService;
            _userManager = userManager;
        }
        [AllowAnonymous]
        [HttpGet("GetAllImages")]
        public async Task<IActionResult> GetAllImagesAsync(Guid userId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound("User Not Found");
            }

            var images = (await _imagesGetterService.GetAllImages(userId)).Select(imgData => new
            {
                imgData.ImageId,
                imgData.UserId,
                imgData.Prompt,
                imgData.IsFavorite,
                Image = File(imgData.Image, "image/png")
            });

            return Ok(images);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateImage([FromBody] PromptRequest request)
        {
            _logger.LogInformation("Received request to generate an image.");
            if (request == null )
                return BadRequest("Invalid request. Please Provide valid data for design.");
            

            if (string.IsNullOrWhiteSpace(request.BrandName))
                return BadRequest("Brand Name Can't be Blank");

            if (string.IsNullOrWhiteSpace(request.BrandStyle))
                return BadRequest("Brand Style Can't be Blank");

            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt Can't be Blank");


            try
            {
                if (_imageGenerator == null)
                    throw new Exception("Image Generator is not initialized.");


                List<ImageData> imageResponses = await _imageGenerator.Generate(request);
                if (imageResponses == null)
                    return BadRequest("Failed to generate image");
                

                var imageResults = imageResponses.Select(img => new
                {
                    img.ImageId,
                    img.UserId,
                    img.Prompt,
                    ImageData = File(img.Image, "image/png")
                });


                return Ok(new { images = imageResults });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error:"+ex.StackTrace);
            }
        }
    }
}

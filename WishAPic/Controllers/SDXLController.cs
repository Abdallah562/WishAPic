using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Http.Results;
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
        private readonly ILogger<SDXLController> _logger;

        public SDXLController(ILogger<SDXLController> logger, IImageGenerator imageGenerator, IImagesGetterService imagesGetterService)
        {
            _imageGenerator = imageGenerator;
            _logger = logger;
            _imagesGetterService = imagesGetterService;
        }
        [AllowAnonymous]    //To be removed
        [HttpGet("GetAllImages")]
        public async Task<IActionResult> GetAllImagesAsync(Guid userId)
        {

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
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                Console.WriteLine("Invalid request received.");
                return BadRequest("Invalid request. Prompt is required.");
            }



            Console.WriteLine($"Received prompt: {request.Prompt}");
            try
            {
                if (_imageGenerator == null)
                {
                    throw new Exception("Image Generator is not initialized.");
                }

                //if (_imageGenerator.IsBrandExist(request.BrandName))
                //    return BadRequest("Brand Name is a Famous. try another one");

                List<ImageData> imageResponses = await _imageGenerator.Generate(request);
                if (imageResponses == null)
                {
                    _logger.LogError("Image generation failed.");
                    return BadRequest("Failed to generate image");
                }

                _logger.LogInformation("Image generated successfully.");
                //foreach (var item in imageResponses)
                //{
                //    _logger.LogError(item.ToString());
                //}
                var imageResults = imageResponses.Select(img => new
                {
                    img.ImageId,
                    img.UserId,
                    img.Prompt,
                    ImageData = File(img.Image, "image/png")
                });
                //return Problem("No");
                //the First image is from the txt-to-img 
                //the others is from Similarity model

                return Ok(new { images = imageResults });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while generating image.");
                return StatusCode(500, "Internal Server Error:"+ex.StackTrace);
            }
        }
    }
}

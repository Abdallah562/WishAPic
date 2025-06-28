using System.Text;
using WishAPic.Controllers;
using WishAPic.Models;
using WishAPic.ServiceContracts;

namespace WishAPic.Services
{

    public class ImageGenerator : IImageGenerator
    {

        private readonly ILogger<ImageGenerator> _logger;
        private readonly HttpClient _httpClient;
        private readonly IImagesAdderService _imagesAdderService;

        public ImageGenerator(HttpClient httpClient, ILogger<ImageGenerator> logger,
            IImagesAdderService imagesAdderService)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(5); // Set the timeout to 5 minutes
            _logger = logger;
            _imagesAdderService = imagesAdderService;
        }
        public async Task<List<ImageData>> Generate(PromptRequest request)
        {
            // Flask endpoint URL
            string txtToImgApiUrl = "https://c5f5-35-247-122-2.ngrok-free.app/generate";

            // Enhance the prompt
            _logger.LogInformation("Before Enhancement: " + request.Prompt);

            var requestData = new
            {
                brandName = request.BrandName,
                brandStyle = request.BrandStyle,
                prompt = request.Prompt
            };

            string jsonRequest = System.Text.Json.JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending to Flask: " + jsonRequest);

            // Send request
            var response = await _httpClient.PostAsync(txtToImgApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                ImageData imageResponse = new ImageData
                {
                    Prompt = request.Prompt,
                    Image = await response.Content.ReadAsByteArrayAsync(),
                    UserId = (Guid)AccountController.CurrentUser!.Id
                };

                await _imagesAdderService.AddImage(imageResponse);
                _logger.LogError(imageResponse.ToString());
                var result = new List<ImageData>() { imageResponse };

                return result;
            }

            _logger.LogError("Flask generation failed with status: " + response.StatusCode);
            return null;
        }
    }

}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WishAPic.Controllers;
using WishAPic.Models;
using WishAPic.Services;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using WishAPic.ServiceContracts;
using WishAPic.DTO;
using WishAPic.Identity;

namespace WishAPic.Services
{
    
    public class ImageGenerator : IImageGenerator
    {

        private readonly ILogger<ImageGenerator> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _flaskApiUrl = "http://127.0.0.1:5000/";
        private readonly IImagesAdderService _imagesAdderService;
        static HashSet<string> brandList;

        public ImageGenerator(HttpClient httpClient, ILogger<ImageGenerator> logger,
            IImagesAdderService imagesAdderService)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(5); // Set the timeout to 5 minutes
            _logger = logger;
            _imagesAdderService = imagesAdderService;
        }
        public async Task<List<ImageData>> SimilarityChecker(ImageData imageResponse)
        {
            //_logger.LogInformation("Before Enhancement: " + request.Prompt);

            _logger.LogError("SimilarityChecking");
            using (var form = new MultipartFormDataContent())
            using (var content = new ByteArrayContent(imageResponse.Image))
            {
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png"); // Adjust based on the actual format
                form.Add(content, "image", "uploaded_image.png");

                HttpResponseMessage response = await _httpClient.PostAsync(_flaskApiUrl+"compare", form);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogError(jsonResponse);
                File.WriteAllText("response.json", jsonResponse);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // ✅ Fix case-sensitive deserialization
                };

                var parsedResponse = System.Text.Json.JsonSerializer.Deserialize<SimilarityCheckerResponse>(jsonResponse,options);

                List<ImageData> imageResonses = new List<ImageData>();
                foreach (var match in parsedResponse.TopMatches)
                {
                    string imagePath = Path.Combine("E:\\Programming\\Projects\\CSharp Projects\\WishAPic\\WishAPic\\Feature Extraction API", match.ImagePath);
                    _logger.LogError(imagePath);
                    if (File.Exists(imagePath))
                    {
                        imageResponse = new ImageData();
                        imageResponse.Image = File.ReadAllBytes(imagePath);
                        imageResonses.Add(imageResponse);
                    }
                    else
                    {
                        Console.WriteLine($"Image not found: {imagePath}");
                    }
                }
                return imageResonses;
            }
        }
        public bool IsBrandExist(string brandName)
        {
            string filePath = @"E:\Programming\Projects\CSharp Projects\WishAPic\WishAPic\Feature Extraction API\famous_brands_500.txt";
            if(brandList == null)
                brandList = new HashSet<string>(File.ReadAllLines(filePath), StringComparer.OrdinalIgnoreCase);

            if (brandList.Contains(brandName))
                return true;

            else
                return false;
        }
        public async Task<List<ImageData>> Generate(PromptRequest request)
        {
            // Flask endpoint URL
            string txtToImgApiUrl = "https://4049-34-16-230-107.ngrok-free.app/generate";

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
                    UserId = (Guid)AccountController.CurrentUser?.Id
                };

                await _imagesAdderService.AddImage(imageResponse);
                _logger.LogError(imageResponse.ToString());
                //var similarItems = await SimilarityChecker(imageResponse);
                var result = new List<ImageData>() { imageResponse };
                //result.AddRange(similarItems);

                return result;
            }

            _logger.LogError("Flask generation failed with status: " + response.StatusCode);
            return null;
        }
    }

}

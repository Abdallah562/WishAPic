using Microsoft.AspNetCore.Http.HttpResults;
using WishAPic.Controllers;
using WishAPic.Data;
using WishAPic.DTO;
using WishAPic.Models;
using WishAPic.ServiceContracts;

namespace WishAPic.Services
{
    public class ImagesAdderService : IImagesAdderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImagesAdderService> _logger;

        public ImagesAdderService(ApplicationDbContext context, ILogger<ImagesAdderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ImageData> AddImage(ImageData imageData)
        {
            await _context.AddAsync(imageData);
            _context.SaveChanges();

            return imageData;
        }

        public ImageData AddToFavorites(ImageData imageData)
        {
            _logger.LogError("Image Before: " + imageData.ToString());
            var existingImage = _context.Images.Find(imageData.ImageId);
            _logger.LogError("Image After: "+existingImage?.ToString());
            if (existingImage == null)
            {
                throw new InvalidOperationException("Image Not found");
            }
            existingImage.IsFavorite = true;
            _context.SaveChanges();
            return imageData;
        }
    }
}

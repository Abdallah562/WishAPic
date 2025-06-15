using WishAPic.Data;
using WishAPic.Models;
using WishAPic.ServiceContracts;

namespace WishAPic.Services
{
    public class ImagesDeleterService : IImagesDeleterService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImagesDeleterService> _logger;
        public ImagesDeleterService(ApplicationDbContext context, ILogger<ImagesDeleterService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ImageData> DeleteFromFavorites(ImageData imageData)
        {
            ImageData image = await _context.Images.FindAsync(imageData.ImageId);
            image.IsFavorite = false;
            _context.SaveChanges();

            return image;
        }

        public ImageData DeleteFromHistory(ImageData imageData)
        {
             _context.Remove(imageData);
            _context.SaveChanges();
            return imageData;
        }
    }
}

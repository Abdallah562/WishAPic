using WishAPic.Controllers;
using WishAPic.Data;
using WishAPic.Identity;
using WishAPic.Models;
using WishAPic.ServiceContracts;

namespace WishAPic.Services
{
    public class ImagesGetterService : IImagesGetterService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImagesGetterService> _logger;


        public ImagesGetterService(ApplicationDbContext context, ILogger<ImagesGetterService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ImageData> FindByIdAsync(Guid imageId)
        {
            return await _context.Images.FindAsync(imageId);
        }

        public async Task<List<ImageData>> GetAllImages(Guid userId)
        {
            _logger.LogError("Get Image:" + userId);

            return await _context.Images.Where(img => img.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<ImageData>> GetFavorites(Guid userId)
        {
            return await _context.Images.Where(img => img.UserId == userId && img.IsFavorite)
                .ToListAsync();
        }
    }
}

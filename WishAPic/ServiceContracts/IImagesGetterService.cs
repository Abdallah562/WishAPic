using WishAPic.Models;

namespace WishAPic.ServiceContracts
{
    public interface IImagesGetterService
    {
        Task<List<ImageData>> GetAllImages(Guid userId);
        Task<List<ImageData>> GetFavorites(Guid userId);
    }


}

using WishAPic.Models;

namespace WishAPic.ServiceContracts
{
    public interface IImagesDeleterService
    {
        ImageData DeleteFromHistory(ImageData imageData);
        Task<ImageData> DeleteFromFavorites(ImageData imageData);
    }
}

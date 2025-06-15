using WishAPic.DTO;
using WishAPic.Models;

namespace WishAPic.ServiceContracts
{
    public interface IImagesAdderService
    {
        Task<ImageData> AddImage(ImageData imageData);
        ImageData AddToFavorites(ImageData imageData);
    }
}

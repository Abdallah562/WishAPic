using WishAPic.Models;

namespace WishAPic.Services
{
    public interface IImageGenerator
    {
        Task<List<ImageData>> Generate(PromptRequest request);

    }
}
using System.Diagnostics;

namespace WishAPic.Models
{
    public class PromptRequest
    {
        public string BrandName { get; set; }
        public string BrandStyle { get; set; }
        public string Prompt { get; set; }
    }
    //public class ImageResponse
    //{
    //    public byte[] Image { get; set; }
    //}

    public class SimilarityCheckerResponse
    {
        public List<TopMatch> TopMatches { get; set; } = new List<TopMatch>();
    }

}

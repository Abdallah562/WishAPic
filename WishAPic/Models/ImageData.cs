
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace WishAPic.Models
{
    public class ImageData
    {
        [Key]
        public Guid ImageId { get; set; }
        public Guid UserId { get; set; }
        public string Prompt { get; set; }
        public byte[] Image { get; set; }
        public bool IsFavorite { get; set; } 

        public override string ToString()
        {
            return
                $"Id: {ImageId}\n" +
                $"UserId: {UserId}\n" +
                $"Prompt: {Prompt}\n" +
                $"Image: {Image}\n";
        }
    }
}

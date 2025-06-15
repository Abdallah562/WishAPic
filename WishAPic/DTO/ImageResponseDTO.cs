namespace WishAPic.DTO
{
    public class ImageResponseDTO
    {
        public int UserId { get; set; }
        public string Prompt { get; set; }
        public byte[] Image { get; set; }
    }
}
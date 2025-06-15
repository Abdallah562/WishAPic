namespace WishAPic.DTO
{
    public class ImageDataDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Prompt { get; set; }
        public string Image { get; set; }
        public override string ToString()
        {
            return
                $"Id: {Id}\n" +
                $"UserId: {UserId}\n" +
                $"Prompt: {Prompt}\n" +
                $"Image: {Image}\n";
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace WishAPic.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        //public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationDateTime { get; set; }
    }
}

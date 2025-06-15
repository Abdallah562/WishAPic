using System.Security.Claims;
using WishAPic.DTO;
using WishAPic.Identity;

namespace WishAPic.ServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponse CreateJwtToken(ApplicationUser user);
        ClaimsPrincipal? GetPrincipalFromJwtToken(string? token); //ClaimsPrincipal represents user details (Id,username,...)
    }
}

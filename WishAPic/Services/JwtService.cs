using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WishAPic.DTO;
using WishAPic.Identity;
using WishAPic.ServiceContracts;

namespace WishAPic.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration configuration;

        public JwtService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public AuthenticationResponse CreateJwtToken(ApplicationUser user)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(
                configuration["Jwt:EXPIRATION_MINUTES"]));

            Claim[] claims =
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()), //Subject (user id)
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()), //JWT unique ID
                new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()), //Issued at (date and time of token generation)
                new Claim(ClaimTypes.NameIdentifier,user.Email), //Unique name identifier of the user (Email)
                new Claim(ClaimTypes.Name,user.FullName), //Name of the User
                new Claim(ClaimTypes.Email,user.Email) //Name of the User
            };

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            SigningCredentials signingCredentials = new
                SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: signingCredentials
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(tokenGenerator);

            return new AuthenticationResponse()
            {
                UserId = user.Id,
                Token = token,
                Email = user.Email,
                FullName = user.FullName,
                Expiration = expiration,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationDateTime = DateTime.Now.AddMinutes(
                    Convert.ToInt32(configuration["RefreshToken:EXPIRATION_MINUTES"]))
            };
        }

        //Creates a refresh token (base 64 string of random numbers)
        private static string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
            var rand = RandomNumberGenerator.Create();

            rand.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token) //Expired Token
        {
            var tokenValidationParameters = new
                TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                ValidateLifetime = false
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new
                JwtSecurityTokenHandler();

            ClaimsPrincipal principal = jwtSecurityTokenHandler
                .ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if(securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }

            return principal;
        }
    }
}

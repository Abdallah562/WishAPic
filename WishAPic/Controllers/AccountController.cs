using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WishAPic.DTO;
using WishAPic.Identity;
using Microsoft.AspNetCore.Identity;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using WishAPic.ServiceContracts;
using System.Security.Claims;

namespace WishAPic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AccountController> _logger;
        internal static ApplicationUser? CurrentUser = null;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager,
            IJwtService jwtService, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApplicationUser>> PostLogin(LoginDTO loginDTO)
        {
            //Validation
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join(" | ",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }



            SignInResult result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password,
                    isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                //Create user
                ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);

                if(user == null)
                    return NoContent();

                await _signInManager.SignInAsync(user, isPersistent: false);
                var authenticationResponse = _jwtService.CreateJwtToken(user);

                user.RefreshToken = authenticationResponse.RefreshToken;
                user.RefreshTokenExpirationDateTime =
                    authenticationResponse.RefreshTokenExpirationDateTime;
                await _userManager.UpdateAsync(user);
                CurrentUser = user;
                //_logger.LogError("UserId: " + user.Id);
                return Ok(authenticationResponse);
            }
            else
            {
                return Problem("Invalid email or password");
            }

        }

        [HttpGet("logout")]
        public async Task<IActionResult> GetLogout()
        {
            await _signInManager.SignOutAsync();
            CurrentUser = null;
            return NoContent();
        }


        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUser>> PostRegister(RegisterDTO registerDTO)
        {
            //_logger.LogError("Register");

            //Validation
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join(" | ",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }

            //Create user
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                FullName = registerDTO.FullName
            };

            IdentityResult result =  await _userManager.CreateAsync(user,registerDTO.Password);

            if (result.Succeeded)
            {
                //Sign-in
                await _signInManager.SignInAsync(user,isPersistent:false);
                var authenticationResponse = _jwtService.CreateJwtToken(user);
                user.RefreshToken = authenticationResponse.RefreshToken;
                user.RefreshTokenExpirationDateTime = 
                    authenticationResponse.RefreshTokenExpirationDateTime;
                await _userManager.UpdateAsync(user);
                CurrentUser = user;

                return Ok(authenticationResponse);
            }
            else
            {
                string errorMessage = string.Join(" | ",
                    result.Errors.Select(e => e.Description));
                return Problem(errorMessage);
            }
        }
        [HttpGet]
        public async Task<IActionResult> IsEmailAlreadyRegister(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Ok(true);

            else
                return Ok(false);
        }

        [HttpPost("generate-new-jwt-token")]
        public async Task<IActionResult> GenerateNewAccessToken(TokenModel tokenModel)
        {
            if (tokenModel == null)
                return BadRequest("Invalid Client request");

            ClaimsPrincipal? principal =  _jwtService.GetPrincipalFromJwtToken(tokenModel.Token);

            if (principal == null)
                return BadRequest("Invalid Jwt Access Token");

            string? email = principal.FindFirstValue(ClaimTypes.Email);

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken ||
                user.RefreshTokenExpirationDateTime <= DateTime.Now)
                return BadRequest("Invalid Refresh Token");

            AuthenticationResponse authenticationResponse = 
                _jwtService.CreateJwtToken(user);

            user.RefreshToken = authenticationResponse.RefreshToken;
            user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;

            await _userManager.UpdateAsync(user);

            return Ok(authenticationResponse);
        }
    }
}

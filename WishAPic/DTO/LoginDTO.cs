using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WishAPic.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be in a proper email address format")]
        [Remote(action: "IsEmailAlreadyRegister", controller: "Account",
            ErrorMessage = "Email is already in use")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password can't be blank")]
        public string Password { get; set; } = string.Empty;
    }
}

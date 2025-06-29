﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WishAPic.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage ="Full Name can't be blank")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be in a proper email address format")]
        [Remote(action: "IsEmailAlreadyRegister", controller: "Account",
            ErrorMessage = "Email is already in use")]
        public string Email { get; set; } = string.Empty;

    //    [Required(ErrorMessage = "Phone Number can't be blank")]
    //    [RegularExpression("^[0-9]*$",ErrorMessage = "Phone number should contain digits only")]
    //    [Remote(action: "IsEmailAlreadyRegister", controller: "Account",
    //ErrorMessage = "Email is already in use")]
    //    public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password can't be blank")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "ConfirmPassword can't be blank")]
        [Compare("Password",ErrorMessage = "Password and ConfirmPassword doesn't match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

﻿using System.ComponentModel.DataAnnotations;

namespace LandScaperMVC.Areas.Admin.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string Name { get; set; }
        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string Surname { get; set; }
        [Required]
        [MaxLength(25)]
        [MinLength(4)]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [MaxLength(250)]
        [MinLength(4)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}

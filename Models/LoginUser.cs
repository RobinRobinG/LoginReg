using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginRegistration.Models
{
    public class LoginUser
    {
        [EmailAddress]
        [Required]
        [Display(Name = "Email:")]
        public string Email {get; set;}

        [DataType(DataType.Password)]
        [Required]
        [Display(Name = "Password:")]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string Password { get; set; }
    }
}
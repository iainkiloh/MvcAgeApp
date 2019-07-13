using System;
using System.ComponentModel.DataAnnotations;

namespace MvcAgeApp.Models
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage ="Please enter your name")]
        public string Name { get; set; }

        public DateTime? Dob { get; set; }

        [Required(ErrorMessage ="Please enter your email")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage =("Please enter a valid email"))]
        public string Email { get; set; }
    }
}

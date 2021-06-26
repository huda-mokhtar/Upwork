using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [MinLength(6, ErrorMessage = "Too short. Use at least 6 characters")]
        [MaxLength(100, ErrorMessage = "Too long. Use 100 characters or less")]
        [RegularExpression(@"[A-Za-z0-9]+@[a-zA-Z]+.[a-zA-Z]{3}", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Too short. Use at least 8 characters")]
        public string Password { get; set; }
    }
}

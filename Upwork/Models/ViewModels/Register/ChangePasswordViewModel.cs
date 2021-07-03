using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Too short. Use at least 8 characters")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Too short. Use at least 8 characters")]
        public string NewPassword { get; set; }

        [Compare("NewPassword",ErrorMessage ="Password doesn't match!")]
        public string ConfirmPassword { get; set; }

        public bool RequiredToSignin { get; set; }
    }
}

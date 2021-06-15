using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class UserData
    {
        [Display(Name = "First name")]
        [Required(ErrorMessage = "First name is required")]
        [MaxLength(100, ErrorMessage = "Too long. Use 100 characters or less")]
        [RegularExpression(@"[a-zA-Z]+", ErrorMessage = "Sorry! No special characters or numbers")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(100, ErrorMessage = "Too long. Use 100 characters or less")]
        [RegularExpression(@"[a-zA-Z]+", ErrorMessage = "Sorry! No special characters or numbers")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Too short. Use at least 8 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public int CountryId { get; set; }

        public string Username { get; set; }

        public bool SendMe { get; set; }

    }
}

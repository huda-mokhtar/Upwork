using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class EditOverviewViewModel
    {
        [Required(ErrorMessage = "Enter your profile overview")]
        [MinLength(100, ErrorMessage = "Too short. An effective overview needs to be at least 100 characters.")]
        public string Overview { get; set; }
    }
}

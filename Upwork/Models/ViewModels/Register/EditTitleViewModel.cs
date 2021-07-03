using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class EditTitleViewModel
    {
        [Required(ErrorMessage = "Enter your profile title")]
        [MinLength(4, ErrorMessage = "A descriptive title must have at least 4 letters.")]
        public string Title { get; set; }
    }
}

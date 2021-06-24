using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class PhoneViewModel
    {
        [Required(ErrorMessage ="This field is required.")]
        public int? Phone { get; set; }

        public int CountryId { get; set; }
    }
}

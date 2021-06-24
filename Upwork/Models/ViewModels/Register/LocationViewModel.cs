using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class LocationViewModel
    {
        [Required(ErrorMessage ="This field is required.")]
        public int? CountryId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string City { get; set; }

        public string ZIP { get; set; }

    }
}

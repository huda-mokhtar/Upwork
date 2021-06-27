using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class ExternalUserData
    {
       
        [Required(ErrorMessage = "Country is required")]
        public int CountryId { get; set; }

        public string Username { get; set; }

        public bool SendMe { get; set; }

        public bool PrivacyPolicy { get; set; }

    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Models.MessageModels;

namespace Upwork.Models
{
    public class ApplicationUser :IdentityUser
    {
        public ApplicationUser()
        {
            Messages = new HashSet<Message>();
        }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Country")]
        [ForeignKey("Country")]
        public int? CountryId { get; set; }
        public string Image { get; set; }

        public virtual Country Country { get; set; }

        [Display(Name = "Yes! Send me genuinely useful emails every now and then to help me get the most out of Upwork.")]
        public bool SendMe { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}

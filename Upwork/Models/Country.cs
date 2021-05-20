using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountryId { get; set; }

        [Display(Name = "Country name")]
        [Required(ErrorMessage = "Country name is required!")]
        [RegularExpression(@"[a-zA-Z]+", ErrorMessage = "Please enter a valid country name")]
        public string Name { get; set; }


        public string FlagImage { get; set; }

        [Required(ErrorMessage = "Country key is required!")]
        public int Key { get; set; }

        public virtual List<ApplicationUser> Users { get; set; }

        public virtual List<City> Cities { get; set; }

        public virtual List<Freelancer_Experience> FreelancerExperiences { get; set; }

    }
}

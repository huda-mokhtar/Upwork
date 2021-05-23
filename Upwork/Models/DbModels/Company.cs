using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyId { get; set; }

        [Display(Name = "Company name")]
        [Required(ErrorMessage = "Company name is required!")]
        public string Name { get; set; }

        public virtual List<Freelancer_Experience> FreelancerExperiences { get; set; }


    }
}

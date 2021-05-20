using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class JobTitle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobTitleId { get; set; }

        [Display(Name = "Job title")]
        [Required(ErrorMessage = "Job title is required!")]
        public string Name { get; set; }

        public virtual List<Freelancer_Experience> FreelancerExperiences { get; set; }

    }
}

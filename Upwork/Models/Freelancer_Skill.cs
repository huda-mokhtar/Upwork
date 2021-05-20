using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class Freelancer_Skill
    {

        [Display(Name = "Freelancer Id")]
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Freelancer")]
        public string FreelancerId { get; set; }

        [Display(Name = "Skill Id")]
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Skill")]
        public int SkillId { get; set; }

        public virtual Freelancer Freelancer { get; set; }

        public virtual Skill Skill { get; set; }

    }
}

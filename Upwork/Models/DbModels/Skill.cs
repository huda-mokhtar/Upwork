using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class Skill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SkillId { get; set; }

        [Display(Name = "Skill name")]
        [Required(ErrorMessage = "Skill name is required!")]
        public string Name { get; set; }

        public virtual List<Freelancer_Skill> Freelancers { get; set; }
    }
}

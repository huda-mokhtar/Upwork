using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Models.DbModels;

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

        [ForeignKey("SubCategory")]
        public int? SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
        public virtual List<Freelancer_Skill> Freelancers { get; set; }
        public List<ProjectSkills> Projects { get; set; }

        public List<JobsSkills> jobsSkills { get; set; }
    }
}

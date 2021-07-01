using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Admin
{
    public class SkillViewModel
    {
        public int? SkillId { get; set; }

        [Display(Name = "Skill name")]
        [Required(ErrorMessage = "Skill name is required!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "SubCategory is required!")]
        public int? SubCategoryId { get; set; }
    }
}

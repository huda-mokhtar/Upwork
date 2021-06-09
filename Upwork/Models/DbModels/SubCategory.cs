using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Models.DbModels;

namespace Upwork.Models
{
    public class SubCategory
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubCategoryId { get; set; }

        [Display(Name = "SubCategory name")]
        [Required(ErrorMessage = "SubCategory name is required!")]
        public string Name { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public virtual List<Freelancer> Freelancers { get; set; }
        public virtual List<Project> Projects { get; set; }
        public virtual List<Skill> Skills { get; set; }

        public virtual List<PostAJob> PostAJobs { get; set; }
    }
}

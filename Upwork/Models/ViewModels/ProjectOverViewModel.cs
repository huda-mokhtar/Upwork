using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Models.DbModels;

namespace Upwork.Models.ViewModels
{
    public class ProjectOverViewModel
    {
        [Required]
        public string Title { get; set; }


        [ForeignKey("SubCategory")]
        public int SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }

        [Required]
        public List<ProjectSkills> Skills { get; set; }

        public List<ProjectTags> Tags { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class ProjectImages
    {
        [Key]
        public int ImgId { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public bool IsCoverd { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        public Project Project { get; set; }

    }
}

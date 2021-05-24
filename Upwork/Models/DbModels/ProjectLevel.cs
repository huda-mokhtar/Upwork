using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class ProjectLevel
    {
        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [ForeignKey("Level")]
        public int LevelId { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int DeleverDays { get; set; }

        public Project Project { get; set; }
        public Level Level { get; set; }
    }
}

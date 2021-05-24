using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class Level
    {
        [Key]
        public int LevelId { get; set; }

        [Required]
        public string Name { get; set; }

        public List<ProjectLevel> Projects { get; set; }
    }
}

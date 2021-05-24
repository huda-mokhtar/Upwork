using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class Tags
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<ProjectTags> Projects { get; set; }
    }
}

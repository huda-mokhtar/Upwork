using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class ProjectTags
    {
        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [ForeignKey("Tags")]
        public int TagsId { get; set; }
        public Project Project { get; set; }
        public Tags Tags { get; set; }
    }
}

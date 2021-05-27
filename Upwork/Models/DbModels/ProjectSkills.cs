using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.DbModels
{
    public class ProjectSkills
    {
        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [ForeignKey("Skill")]
        public int SkillId { get; set; }

        public Project Project { get; set; }
        public Skill Skill { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.DbModels
{
    public class JobsSkills
    {
        [ForeignKey("Jobs")]
        public int JobsId { get; set; }
        public Jobs Jobs { get; set; }
        
        [ForeignKey("skill")]
        public int skillId { get; set; }
        public Skill skill { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.DbModels
{
    public class JobSkills
    {
        public int PostAJobId { get; set; }
        public PostAJob PostAJob { get; set; }
        public int skillId { get; set; }
        public Skill skill { get; set; }
    }
}

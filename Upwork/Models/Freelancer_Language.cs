using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class Freelancer_Language
    {
        [Column(Order = 0)]
        [ForeignKey("Freelancer")]
        public string FreelancerId { get; set; }

        public virtual Freelancer Freelancer { get; set; }

        [Column(Order = 1)]
        [ForeignKey("Language")]
        public int LanguageId { get; set; }

        public virtual Language Language { get; set; }

        [ForeignKey("Proficiency")]
        public int ProficiencyId { get; set; }

        public virtual Language_Proficiency Proficiency { get; set; }
    }
}

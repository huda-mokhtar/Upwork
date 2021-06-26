using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.DbModels
{
    public class Freelancer_Job
    {
        [ForeignKey("Jobs")]
        public int JobsId { get; set; }
      
        [ForeignKey("Freelancer")]
        public string FreelancerId { get; set; }

        public bool? IsSaved { get; set; } = false;
        public bool? IsProposal { get; set; } = false;
        public bool? Isdislike { get; set; } = false;

        [Range(0, 5)]
        public int Rate { get; set; }
        public Jobs Jobs { get; set; }

        public Freelancer Freelancer { get; set; }
    }
}

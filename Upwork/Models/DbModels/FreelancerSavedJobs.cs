using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.DbModels
{
    public class FreelancerSavedJobs
    {
        [ForeignKey("Jobs")]
        public int JobsId { get; set; }
        public Jobs Jobs { get; set; }

        [ForeignKey("Freelancer")]
        public string FreelancerId { get; set; }
        public Freelancer Freelancer { get; set; }
    }
}

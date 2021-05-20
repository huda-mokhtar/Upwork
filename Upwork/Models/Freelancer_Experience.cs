using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class Freelancer_Experience
    {
        [Column(Order = 0)]
        [ForeignKey("Freelancer")]
        public string FreelancerId { get; set; }

        public virtual Freelancer Freelancer { get; set; }

        [Column(Order = 1)]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }

        [Column(Order = 2)]
        [ForeignKey("Country")]
        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        [Column(Order = 3)]
        [ForeignKey("JobTitle")]
        public int JobTitleId { get; set; }

        public virtual JobTitle JobTitle { get; set; }


        public string Location { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string Description { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class Freelancer_Education
    {
       
        [Column(Order = 0)]
        [ForeignKey("Freelancer")]
        public string FreelancerId { get; set; }

        public virtual Freelancer Freelancer { get; set; }

        
        [Column(Order = 1)]
        [ForeignKey("School")]
        public int SchoolId { get; set; }

        public virtual School School { get; set; }

       
        [Column(Order = 2)]
        [ForeignKey("AreaOfStudy")]
        public int AreaId { get; set; }

        public virtual AreaOfStudy AreaOfStudy { get; set; }

       
        [Column(Order = 3)]
        [ForeignKey("Degree")]
        public int DegreeId { get; set; }

        public virtual Degree Degree { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string Description { get; set; }

        }
    }


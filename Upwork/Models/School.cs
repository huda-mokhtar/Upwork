using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class School
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SchoolId { get; set; }


        [Display(Name = "School name")]
        [Required(ErrorMessage = "School name is required!")]
        public string Name { get; set; }

        public virtual List<Freelancer_Education> FreelancerEducations { get; set; }



    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class Degree
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DegreeId { get; set; }


        [Display(Name = "Degree name")]
        [Required(ErrorMessage = "Degree name is required!")]
        public string Name { get; set; }

        public virtual List<Freelancer_Education> FreelancerEducations { get; set; }

    }
}

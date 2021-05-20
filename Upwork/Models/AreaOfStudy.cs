using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class AreaOfStudy
    {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int AreaId { get; set; }


            [Display(Name = "Area of study")]
            [Required(ErrorMessage = "Area of study name is required!")]
            public string Name { get; set; }

            public virtual List<Freelancer_Education> FreelancerEducations { get; set; }

        }
    }


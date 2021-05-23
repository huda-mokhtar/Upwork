using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class Language
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LanguageId { get; set; }

        [Display(Name = "Language")]
        [Required(ErrorMessage = "Language name is required!")]
        public string Name { get; set; }

        public virtual List<Freelancer_Language> FreelancerLanguages { get; set; }

    }
}

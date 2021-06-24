using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class LanguagesViewModel
    {
        [Required(ErrorMessage = "You must select proficiency level.")]
        public int? ProficiencyId { get; set; }

        public int? Language1Id { get; set; }

        public int? Proficiency1Id { get; set; }


    }
}

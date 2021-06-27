using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class AddEducationViewModel
    {
        [Required(ErrorMessage ="This field is required.")]
        public string School { get; set; }

        public string AreaOfStudy { get; set; }

        public string Degree { get; set; }

        public int? From { get; set; }

        public int? To { get; set; }

        public string Description { get; set; }

        public string FreerlancerId { get; set; }

        public int? AreaId { get; set; }

        public int? SchoolId { get; set; }

        public int? DegreeId { get; set; }

    }
}

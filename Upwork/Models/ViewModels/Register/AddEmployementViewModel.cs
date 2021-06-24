using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class AddEmployementViewModel
    {
        [Required]
        public string Company { get; set; }

        public int? CountryId { get; set; }

        public string Location { get; set; }

        public string Title { get; set; }

        public int FromMonth { get; set; }

        public int ToMonth { get; set; }

        public int FromYear { get; set; }

        public int ToYear { get; set; }

        public string Description { get; set; }
    }
}

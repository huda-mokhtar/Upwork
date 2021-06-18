using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class HourlyRateViewModel
    {
        [Required(ErrorMessage ="Enter your rate")]
        [Range(3,maximum:999,ErrorMessage ="Please enter a value between $3.00 and $999.00")]
        public float HourlyRate { get; set; }
    }
}

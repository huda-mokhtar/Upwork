using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Admin
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage ="Role name is required!")]
        public string Name { get; set; }
    }
}

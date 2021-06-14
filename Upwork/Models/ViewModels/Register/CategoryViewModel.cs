using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class CategoryViewModel
    {
        [Display(Name ="Category")]
        [Required(ErrorMessage ="You must select a category. ")]
        public int CategoryId { get; set; }

        [Display(Name = "Subcategory")]
        [Required(ErrorMessage = "You must select a subcategory. ")]
        public int SubCategoryId { get; set; }

    }
}

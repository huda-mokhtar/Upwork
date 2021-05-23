using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Display(Name = "Category name")]
        [Required(ErrorMessage = "Category name is required!")]
        public string Name { get; set; }

        public virtual List<SubCategory> SubCategories { get; set; }

        public virtual List<Freelancer> Freelancers { get; set; }

    }
}
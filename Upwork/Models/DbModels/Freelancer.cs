using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Models.DbModels;

namespace Upwork.Models
{
    public class Freelancer
    {
       
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("User")]
        public string FreelancerId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

        public virtual Category Category { get; set; }

        [ForeignKey("SubCategory")]
        public int? SubCategoryId { get; set; }

        public virtual SubCategory SubCategory { get; set; }

        public virtual List<Freelancer_Skill> Skills { get; set; }

        public String ExperienceLevel { get; set; }

        public float? HourlyRate { get; set; }

        public string Title { get; set; }

        public string Overview { get; set; }

        public string Image { get; set; }

        [ForeignKey("City")]
        public int? CityId { get; set; }

        public virtual City City { get; set; }

        public string Street { get; set; }

        public string ZIP { get; set; }

        public string PhoneNumber { get; set; }

        [ForeignKey("PhoneCountry")]
        public int? PhoneCountryId { get; set; }

        public virtual Country PhoneCountry { get; set; }

        public string VideoLink { get; set; }

        public virtual List<Freelancer_Education> Educations { get; set; }

        public virtual List<Freelancer_Experience> Experiences { get; set; }

        public virtual List<Freelancer_Language> Languages { get; set; }
        
        public virtual List<FreelancerSavedJobs> FreelancerSavedJobs { get; set; }

        public List<Project> Projects { get; set; }

    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Upwork.Models;
using Upwork.Models.DbModels;

namespace Upwork.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<AreaOfStudy> AreasOfStudy { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<City> Cities { get; set; }

        public virtual DbSet<Company> Companies { get; set; }

        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<Degree> Degrees { get; set; }

        public virtual DbSet<Freelancer> Freelancers { get; set; }

        public virtual DbSet<Freelancer_Education> Freelancer_Education { get; set; }

        public virtual DbSet<Freelancer_Experience> Freelancer_Experience { get; set; }

        public virtual DbSet<Freelancer_Language> Freelancer_Language { get; set; }

        public virtual DbSet<Freelancer_Skill> Freelancer_Skill { get; set; }

        public virtual DbSet<JobTitle> JobTitle { get; set; }

        public virtual DbSet<Language> Languages { get; set; }

        public virtual DbSet<Language_Proficiency> Language_Proficiency { get; set; }

        public virtual DbSet<School> Schools { get; set; }

        public virtual DbSet<Skill> Skills { get; set; }

        public virtual DbSet<SubCategory> SubCategories { get; set; }

        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectImages> ProjectImages { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<ProjectLevel> ProjectLevels { get; set; }
        public virtual DbSet<ProjectQuestion> ProjectQuestions { get; set; }
        public virtual DbSet<ProjectSteps> ProjectSteps { get; set; }
        public virtual DbSet<Tags> Tags { get; set; }
        public virtual DbSet<ProjectTags> ProjectTags { get; set; }
        public virtual DbSet<ProjectSkills> ProjectSkills { get; set; }


        public virtual DbSet<PostAJob> PostAJobs { get; set; }
        public virtual DbSet<JobSkills> JobSkills { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Freelancer_Education>().HasKey(o => new { o.SchoolId, o.DegreeId, o.AreaId, o.FreelancerId });
            builder.Entity<Freelancer_Experience>().HasKey(o => new { o.JobTitleId, o.CountryId, o.CompanyId, o.FreelancerId });
            builder.Entity<Freelancer_Language>().HasKey(o => new { o.LanguageId, o.FreelancerId });
            builder.Entity<Freelancer_Skill>().HasKey(o => new { o.SkillId, o.FreelancerId });
            builder.Entity<ProjectLevel>().HasKey(o => new { o.ProjectId, o.LevelId });
            builder.Entity<ProjectTags>().HasKey(o => new { o.ProjectId, o.TagsId });
            builder.Entity<ProjectSkills>().HasKey(o => new { o.ProjectId, o.SkillId });
            builder.Entity<JobSkills>().HasKey(o => new { o.skillId, o.PostAJobId });

        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Data;
using Upwork.Models;

namespace Upwork.services
{
    public interface IProject
    {
        Project Create(Project project);
    }
    public class ProjectDB : IProject
    {
        private ApplicationDbContext db ;
        public Project Create(Project project)
        {
            db.Projects.Add(project);
            db.SaveChanges();
            return project;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Projects
{
    public class ProjectDescription
    {
        public string Description { get; set; }
        public List<string> Questions { get; set; }
        public List<string> Answers { get; set; }
        public List<ProjectSteps> Steps { get; set; }

    }
}

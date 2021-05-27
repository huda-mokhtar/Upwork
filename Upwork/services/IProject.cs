using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Models;

namespace Upwork.services
{
    public interface IProject
    {
        Project Create(Project project);
    }
}

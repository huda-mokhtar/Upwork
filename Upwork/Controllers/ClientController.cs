using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Data;

namespace Upwork.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PostJob()
        {
            /*ViewData["SubCategory"] = new SelectList(_context.SubCategories, "SubCategoryId", "Name");
            if (HttpContext.Session.GetString("JobId") != null)
            {
                var JobId = int.Parse(HttpContext.Session.GetString("JobId"));
                var job = _context.PostAJobs.FirstOrDefault(a => a.Id == JobId);
                return View(job);
            }*/
            return View();
        }
        public IActionResult PostJobTitle()
        {
            return View();
        }
        public IActionResult PostJobSkills()
        {
            return View();
        }
        public IActionResult PostJobScope()
        {
            return View();
        }
        public IActionResult PostJobBudget()
        {
            return View();
        }
    }
}

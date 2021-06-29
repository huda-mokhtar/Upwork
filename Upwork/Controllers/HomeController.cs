using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Data;
using Upwork.Models;

namespace Upwork.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Profile(string id)
        {
            var freelancer = await _context.Freelancers.Include(a => a.Educations).Include(a => a.SubCategory).Include(a => a.Category).Include(a => a.Freelancer_Jobs).Include(a => a.User).Include(a => a.City).Include(a => a.Skills).Include(a => a.Languages).FirstOrDefaultAsync(a => a.FreelancerId ==id);

            if (freelancer == null)
            {
                return NotFound();
            }

            ViewData["countries"] = _context.Countries.ToList();
            ViewData["Languages"] = _context.Freelancer_Language.Where(a => a.FreelancerId == freelancer.FreelancerId).Include(a => a.Language).Include(a => a.Proficiency).ToList();
            ViewData["Skills"] = _context.Skills.ToList();
            ViewData["Education"] = _context.Freelancer_Education.Where(a => a.FreelancerId == freelancer.FreelancerId).Include(a => a.AreaOfStudy).Include(a => a.Degree).Include(a => a.School).ToList();
            ViewData["Experience"] = _context.Freelancer_Experience.Where(a => a.FreelancerId == freelancer.FreelancerId).Include(a => a.Company).Include(a => a.Country).Include(a => a.JobTitle).ToList();

            return View(freelancer);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

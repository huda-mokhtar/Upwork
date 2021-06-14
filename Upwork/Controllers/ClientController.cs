using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Data;
using Upwork.Models.DbModels;

namespace Upwork.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostenviroment;

        public ClientController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostenviroment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PostJob()
        {
            if (HttpContext.Session.GetString("JobId") != null)
            {
                var JobId = int.Parse(HttpContext.Session.GetString("JobId"));
                var Job = _context.Jobs.FirstOrDefault(a => a.Id == JobId);
                return View(Job);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task< IActionResult> PostJob(Jobs job)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("JobId") != null)
                {
                    var jobId = int.Parse(HttpContext.Session.GetString("JobId"));
                    var jobOld = _context.Jobs.FirstOrDefault(a => a.Id == jobId);
                    jobOld.Type = job.Type;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(PostJobTitle));
                }
                else
                {
                    _context.Add(job);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("JobId", job.Id.ToString());
                    return RedirectToAction(nameof(PostJobTitle));
                }
            }
            return View(job);
        }

        public IActionResult PostJobTitle()
        {
            ViewData["SubCategory"] = _context.SubCategories;
            ViewData["Category"]= new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }
        public async Task<IActionResult> GetSubCategories(int Id)
        {
            var SubCategoryList = _context.SubCategories.Where(a => a.CategoryId == Id);
            return Json(SubCategoryList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> PostJobTitle(Jobs job)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("JobId") != null)
                {
                    var jobId = int.Parse(HttpContext.Session.GetString("JobId"));
                    var Job = _context.Jobs.FirstOrDefault(s => s.Id == jobId);
                    Job.Title = job.Title;
                    Job.subCategoryId = job.subCategoryId;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(PostJobSkills));
                }
                else
                {
                    return RedirectToAction(nameof(PostJob));
                }
            }
            return View(job);
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
        public IActionResult ReviewJobPosting()
        {
            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Upwork.Data;
using Upwork.Models;
using Upwork.Models.ViewModels;
using Upwork.services;

namespace Upwork.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Projects.Include(p => p.Freelancer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Freelancer)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers, "FreelancerId", "FreelancerId");
            ViewData["SubCategory"]= new SelectList(_context.SubCategories, "SubCategoryId", "Name");
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Project project)
        {
            if (ModelState.IsValid)
            {
                project.FreelancerId = "a123";
  
                _context.Add(project);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("ProjectId",project.ProjectId.ToString());
                return RedirectToAction(nameof(Gallary));
            }
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers, "FreelancerId", "FreelancerId", project.FreelancerId);
            return View(project);
        }
        //Get:Skills
        public async Task<IActionResult> GetSkills(int Id)
        {
            var SkillsList = _context.Skills.Where(a => a.SubCategoryId ==Id);
            return PartialView(SkillsList.ToList());
        }

        //Get:Gallary{
        public async Task<IActionResult> Gallary()
        {
            return View();
        }

        //Get Description
        public async Task<IActionResult> Description()
        {
            return View();
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers, "FreelancerId", "FreelancerId", project.FreelancerId);
            return View(project);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,Title,Description,Requierments,SimultaneousProjects,FreelancerId")] Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers, "FreelancerId", "FreelancerId", project.FreelancerId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Freelancer)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET:Projects/Pricing
        [HttpGet]
        [Route("Projects/pricing")]
        public async Task<IActionResult> CreatePrice() //(ProjectOverViewModel model)
        {
            //ViewData["OverView"] = ModelBinderAttribute;
            List < Level > levels =  _context.Levels.ToList();
            // ViewBag["Levels"] = levels[1];
            ViewData["Levels"] = _context.Levels.ToList();
            return View();
        }
        
        //GET:Projects/Pricing
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Projects/pricing")]
        public async Task<IActionResult> CreatePrice(ProjectLevel model)
        {
            ViewData["Levels"] =  _context.Levels.ToList();
            return View();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }

        
    }
}

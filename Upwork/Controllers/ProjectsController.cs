using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Upwork.Data;
using Upwork.Models;
using Upwork.Models.ViewModels;
using Upwork.Models.ViewModels.Projects;
using Upwork.services;
using Microsoft.AspNetCore.Hosting;


namespace Upwork.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostenviroment;

        public ProjectsController(ApplicationDbContext context,IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostenviroment = hostingEnvironment; 
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
                return RedirectToAction(nameof(CreatePrice));
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

        //GET:Projects/Pricing
        [Route("Projects/pricing")]
        public async Task<IActionResult> CreatePrice() 
        {
            //ViewData["OverView"] = ModelBinderAttribute;
            return View();
        }

        //POST:Projects/Pricing
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Projects/pricing")]
        public async Task<IActionResult> CreatePrice(Project model)
        {
                
                if (HttpContext.Session.GetString("ProjectId") != null)
                {
                    var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
                    var project = _context.Projects.FirstOrDefault(s => s.ProjectId == projectId);
                    project.StarterDeliverDays = model.StarterDeliverDays;
                    project.StandardDeliverDays = model.StandardDeliverDays;
                    project.AdvancedDeliverDays = model.AdvancedDeliverDays;
                    project.StarterPrice = model.StarterPrice;
                    project.StandardPrice = model.StandardPrice;
                    project.AdvancedPrice = model.AdvancedPrice;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Gallary));
                }
                else
                {
                    return RedirectToAction(nameof(Create));
                }
        }

        //Get:Gallary{
        public async Task<IActionResult> Gallary()
        {
            return View();
        }
        //Post Gallary
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gallary(ImageModel model)
        {
            if (ModelState.IsValid)
            {
                var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
                if (HttpContext.Session.GetString("ProjectId") != null)
                {
                    string UniqueFileName = null;
                    string photoName = null;
                    string folder = Path.Combine(_hostenviroment.WebRootPath, "images");
                    photoName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string photoPath = Path.Combine(folder, photoName);
                    model.Image.CopyTo(new FileStream(photoPath, FileMode.Create));
                    var project = _context.Projects.FirstOrDefault(s => s.ProjectId == projectId);
                    //model = prject
                    project.Image = photoName;
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Description));
                }
                else
                {
                    return RedirectToAction(nameof(Create));
                }
            }
            return View();
        }

        //Get Description
        public async Task<IActionResult> Description()
        {
            return View();
        }
        //Post Description
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Description(Project project)
        {
            return RedirectToAction(nameof(Create));
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

       
        //GET:Projects/Requierments
        [HttpGet]
        [Route("Projects/Requierments")]
        public async Task<IActionResult> Requierment() 
        {
            return View();
        }
        
        //GET:Projects/Requierments
        [HttpGet]
        [Route("Projects/Review")]
        public async Task<IActionResult> ReviewProject() 
        {
            return View();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }

        
    }
}

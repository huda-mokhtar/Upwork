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
using Upwork.Models.DbModels;

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


        // GET: Projects/CreateOverView
        public IActionResult Create()
        {
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers, "FreelancerId", "FreelancerId");
            ViewData["SubCategory"]= new SelectList(_context.SubCategories, "SubCategoryId", "Name");
            if (HttpContext.Session.GetString("ProjectId") != null)
            {
                var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
               var project = _context.Projects.FirstOrDefault(a => a.ProjectId == projectId);
                return View(project);
            }
                return View();
        }
        // POST: Projects/CreateOverView
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Project project, Dictionary<string, bool> Skills)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("ProjectId") != null)
                {
                    var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
                    var projectOld=_context.Projects.FirstOrDefault(a => a.ProjectId == projectId);
                    projectOld.Title = project.Title;
                    projectOld.SubCategoryId = project.SubCategoryId;
               
                    await _context.SaveChangesAsync();
                    foreach (KeyValuePair<string, bool> item in Skills)
                    {
                        if (item.Value == true)
                        {
                            var skills=_context.ProjectSkills.FirstOrDefault(a => a.ProjectId == projectId);
                            skills.SkillId = int.Parse(item.Key);
                            await _context.SaveChangesAsync();
                        }
                    }
                    return RedirectToAction(nameof(CreatePrice));
                }
                else{
                    project.FreelancerId = "a123";

                    _context.Add(project);
                    await _context.SaveChangesAsync();
                    foreach (KeyValuePair<string, bool> item in Skills)
                    {
                        if (item.Value == true)
                        {
                            ProjectSkills skill = new ProjectSkills() { ProjectId = project.ProjectId, SkillId = int.Parse(item.Key) };
                            _context.ProjectSkills.Add(skill);
                            await _context.SaveChangesAsync();
                        }
                    }
                    HttpContext.Session.SetString("ProjectId", project.ProjectId.ToString());
                    return RedirectToAction(nameof(CreatePrice));
                }
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
            if (HttpContext.Session.GetString("ProjectId")!=null)
            {
                var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == projectId);
                if (project.StarterPrice != null)
                {
                    return View(project);
                }
            }
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
            if (HttpContext.Session.GetString("ProjectId") != null)
            {
                var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == projectId);
                if (project.Image != null)
                {
                    var projectImage = new ImageModel();
                    projectImage.imageName = project.Image;
                    return View(projectImage);
                }
            }
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
            if (HttpContext.Session.GetString("ProjectId") != null)
            {
                var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == projectId);
                if (project.Description != null)
                {
                    return View(project);
                }
            }
            return View();
        }

        //Post Description
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Description(Project model)
        {
            if (HttpContext.Session.GetString("ProjectId") != null)
            {
                var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
                
                    var project= _context.Projects.FirstOrDefault(a => a.ProjectId == projectId);
                    project.Description = model.Description;
                    project.QuestionContent = model.QuestionContent;
                    project.QuestionContent = model.QuestionAnswer;
                    project.StepName = model.StepName;
                    project.StepDescription = model.StepDescription;
                     _context.SaveChanges();
                    return RedirectToAction(nameof(Requierment));
            }
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


        public async Task<IActionResult> Cancel(Project project)
        {
            if (HttpContext.Session.GetString("ProjectId") != null)
            {
                HttpContext.Session.Remove("ProjectId"); 
            }
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Requierment() 
        {
            if (HttpContext.Session.GetString("ProjectId") != null)
            {
                var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == projectId);
                if (project.Requierments != null)
                {
                   
                    return View(project);
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Requierment(Project model)
        {

            if (HttpContext.Session.GetString("ProjectId") != null)
            {
                var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
                model.ProjectId = projectId;
                  
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == projectId);
                project.Requierments = model.Requierments;
                _context.SaveChanges();
                return RedirectToAction(nameof(ReviewProject));
            }
            else
            {
                return RedirectToAction(nameof(Create));
            }
            
        }
        
        //GET:Projects/Requierments
        [Route("Projects/Review")]
        public async Task<IActionResult> ReviewProject() 
        {
            if (HttpContext.Session.GetString("ProjectId") != null)
            {
                var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == projectId);
                if (project.SimultaneousProjects != null)
                {
                    return View(project);
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewProject(Project model)
        {
            if (HttpContext.Session.GetString("ProjectId") != null)
            {
                var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
                model.ProjectId = projectId;
                    var project = _context.Projects.FirstOrDefault(a => a.ProjectId == projectId);
                    project.SimultaneousProjects = model.SimultaneousProjects;
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
            }
            else{
                    return RedirectToAction(nameof(Create));
            }
        }
        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }

        
    }
}

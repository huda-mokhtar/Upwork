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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Upwork.Controllers
{
    [Authorize(Roles = "Freelancer")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostenviroment;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(ApplicationDbContext context,IHostingEnvironment hostingEnvironment,UserManager<ApplicationUser> usermanager)
        {
            _context = context;
            _hostenviroment = hostingEnvironment;
            _userManager = usermanager;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var CurrentUser = await _userManager.GetUserAsync(User);
            var Projects =  _context.Projects.Where(a => a.IsDraft == false && a.FreelancerId==CurrentUser.Id).ToList();
             
            var Drafts =  _context.Projects.Where(a => a.IsDraft == true &&a.FreelancerId == CurrentUser.Id).ToList();
            ViewData["DraftProject"] = Drafts;
            return View(Projects);
        }


        // GET: Projects/CreateOverView
        public IActionResult Create(int id)
        {  ViewData["id"] = id;
            ViewData["FreelancerId"] = new SelectList(_context.Freelancers, "FreelancerId", "FreelancerId");
            ViewData["SubCategory"]= new SelectList(_context.SubCategories, "SubCategoryId", "Name");
            if (id != 0)
            {
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == id);
                return View(project);
            }
                return View();
        }
        // POST: Projects/CreateOverView
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Project project, Dictionary<string, bool> Skills)
        {
            var CurrentUser = await _userManager.GetUserAsync(User);
            var OldProject = _context.Projects.FirstOrDefault(a => a.ProjectId == project.ProjectId);
            if (OldProject != null) 
            { 
                OldProject.Title = project.Title;
                OldProject.SubCategoryId = project.SubCategoryId;
                await _context.SaveChangesAsync();
                var skills=_context.ProjectSkills.FirstOrDefault(a => a.ProjectId == OldProject.ProjectId);
                foreach (KeyValuePair<string, bool> item in Skills)
                {
                    if (item.Value == true)
                    { 
                        skills.SkillId = int.Parse(item.Key);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction(nameof(CreatePrice),new {id=project.ProjectId });
            }
            else{
                project.FreelancerId = CurrentUser.Id;
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
                ViewData["FreelancerId"] = new SelectList(_context.Freelancers, "FreelancerId", "FreelancerId", project.FreelancerId);
                return RedirectToAction(nameof(CreatePrice), new { id = project.ProjectId });

            }
        }
        //Get:Skills
        public async Task<IActionResult> GetSkills(int Id)
        {
            var SkillsList = _context.Skills.Where(a => a.SubCategoryId ==Id);
            return PartialView(SkillsList.ToList());
        }

        //GET:Projects/Pricing
        [Route("Projects/pricing")]
        public async Task<IActionResult> CreatePrice(int id) 
        {
            ViewData["id"] = id;
            if (id!=0)
            {
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == id);
                if (project.StarterPrice != null)
                {
                    return View(project);
                }
                return View();
            }
            return RedirectToAction(nameof(Create));
        }

        //POST:Projects/Pricing
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Projects/pricing")]
        public async Task<IActionResult> CreatePrice(Project model)
        {
                var project = _context.Projects.FirstOrDefault(s => s.ProjectId == model.ProjectId);
                if (project != null)
                {
                    project.StarterDeliverDays = model.StarterDeliverDays;
                    project.StandardDeliverDays = model.StandardDeliverDays;
                    project.AdvancedDeliverDays = model.AdvancedDeliverDays;
                    project.StarterPrice = model.StarterPrice;
                    project.StandardPrice = model.StandardPrice;
                    project.AdvancedPrice = model.AdvancedPrice;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Gallary), new { id = model.ProjectId });
                }
                return RedirectToAction(nameof(Create));
           
        }

        //Get:Gallary{
        public async Task<IActionResult> Gallary(int id)
        {
            ViewData["id"] = id;
            if (id != 0)
            {
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == id);
                if (project.Image != null)
                {
                    var projectImage = new ImageModel();
                    projectImage.imageName = project.Image;
                    return View(projectImage);
                }
                return View();
            }
            return RedirectToAction(nameof(Create));
        }
        //Post Gallary
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gallary( ImageModel model)
        {
            if (ModelState.IsValid)
            { 
                    string UniqueFileName = null;
                    string photoName = null;
                    string folder = Path.Combine(_hostenviroment.WebRootPath, "images");
                    photoName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string photoPath = Path.Combine(folder, photoName);
                    model.Image.CopyTo(new FileStream(photoPath, FileMode.Create));
                    var project = _context.Projects.FirstOrDefault(s => s.ProjectId == model.id);
                if (project != null)
                {
                    //model = prject
                    project.Image = photoName;
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Description), new { id = model.id });
                }
                return RedirectToAction(nameof(Create));
                
            }
            ViewData["id"] = model.id;
            return View();
        }

        //Get Description
        public async Task<IActionResult> Description(int id)
        {
            ViewData["id"] = id;
            if (id != 0)
            {
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == id);
                if (project.Description != null)
                {
                    return View(project);
                }
                return View();
            }
            return RedirectToAction(nameof(Create));
        }

        //Post Description
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Description(Project model)
        {
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == model.ProjectId);
                if (project!= null)
                {
                    project.Description = model.Description;
                    project.QuestionContent = model.QuestionContent;
                    project.QuestionContent = model.QuestionAnswer;
                    project.StepName = model.StepName;
                    project.StepDescription = model.StepDescription;
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Requierment), new { id = model.ProjectId });
                }
                return RedirectToAction(nameof(Create));
           
        }
        //GET:Projects/Requierments
        public async Task<IActionResult> Requierment(int id)
        {
            ViewData["id"] = id;
            if (id != 0)
            {
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == id);
                if (project.Requierments != null)
                {
                    return View(project);
                }
                return View();
            }
            return RedirectToAction(nameof(Create));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Requierment(Project model)
        {
            var project = _context.Projects.FirstOrDefault(a => a.ProjectId == model.ProjectId);
            if (project != null) {
                project.Requierments = model.Requierments;
                _context.SaveChanges();
                return RedirectToAction(nameof(ReviewProject), new { id = model.ProjectId });
            } 
            return RedirectToAction(nameof(Create), new { id = model.ProjectId });
            
        }

        //GET:Projects/ReviewProject
        public async Task<IActionResult> ReviewProject(int id)
        {
            ViewData["id"] = id;
            if (id != 0)
            {
                var project = _context.Projects.FirstOrDefault(a => a.ProjectId == id);
                if (project.SimultaneousProjects != null)
                {
                    return View(project);
                }
                return View();
            }
            return RedirectToAction(nameof(Create));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewProject(Project model)
        {
            var project = _context.Projects.FirstOrDefault(a => a.ProjectId == model.ProjectId);
            if (project != null)
            {
                project.SimultaneousProjects = model.SimultaneousProjects;
                project.IsDraft = false;
                project.Date = DateTime.Now;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Create), new { id = model.ProjectId });
            
        }
        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var project= _context.Projects.FirstOrDefault(a=>a.ProjectId==id);
            if (project == null)
            {
                return View("Error");
            }
            return RedirectToAction(nameof(Create),new {id=project.ProjectId });
        }
        public async Task<IActionResult> Cancel(Project project)
        {
                return RedirectToAction(nameof(Index));
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Projects/ProjectDetails/5
        public async Task<IActionResult> ProjectDetails(int id)
        {
            var Project = _context.Projects.Include(a => a.Skills).Include(a=>a.SubCategory).FirstOrDefault(a => a.ProjectId == id);
            return View(Project);
        }
       private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }

        
    }
}

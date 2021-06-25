using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Data;
using Upwork.Models;
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
            List<Jobs> alljobs = new List<Jobs>(_context.Jobs);
            //alljobs.Reverse();
            //ViewData["DraftedJobs"] = _context.Jobs.Where(a => a.IsDraft == true);
            //ViewData["postedJobs"] = _context.Jobs.Where(a => a.IsDraft == false);
            return View(alljobs);
        }
        public IActionResult ReusePosting(int id)
        {
            HttpContext.Session.SetString("reuseJob", id.ToString());
            return RedirectToAction(nameof(PostJobTitle));
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
        public async Task<IActionResult> PostJob(Jobs job)
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
                    HttpContext.Session.SetString("JobType", job.Type.ToString());
                    return RedirectToAction(nameof(PostJobTitle));
                }
            }
            return View(job);
        }

        public IActionResult PostJobTitle()
        {
                if (HttpContext.Session.GetString("JobId") != null)
                {
                    var jobId = int.Parse(HttpContext.Session.GetString("JobId"));
                    var jobOld = _context.Jobs.FirstOrDefault(a => a.Id == jobId);
                    if (jobOld.subCategoryId != null)
                    {
                        var categoryId = (_context.SubCategories.FirstOrDefault(a => a.SubCategoryId == jobOld.subCategoryId)).CategoryId;
                        //ViewData["categoryId"] = categoryId;
                        ViewData["SubCategory"] = _context.SubCategories;
                        ViewData["Categoryid"] = categoryId;
                        ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "Name", categoryId);
                        return View(jobOld);
                    }
                    else
                    {
                        ViewData["SubCategory"] = _context.SubCategories;
                        ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "Name");
                        return View(jobOld);
                    }

                }
                else if (HttpContext.Session.GetString("reuseJob") != null)
                {
                    var reusejobId = int.Parse(HttpContext.Session.GetString("reuseJob"));
                    var reusejobOld = _context.Jobs.FirstOrDefault(a => a.Id == reusejobId);
                    if (reusejobOld.subCategoryId != null)
                    {
                        var categoryId = (_context.SubCategories.FirstOrDefault(a => a.SubCategoryId == reusejobOld.subCategoryId)).CategoryId;
                        //ViewData["categoryId"] = categoryId;
                        ViewData["SubCategory"] = _context.SubCategories;
                        ViewData["Categoryid"] = categoryId;
                        ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "Name", categoryId);
                        return View(reusejobOld);
                    }
                    else
                    {
                        ViewData["SubCategory"] = _context.SubCategories;
                        ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "Name");
                        return View(reusejobOld);
                    }
                }
                else
                {
                    ViewData["SubCategory"] = _context.SubCategories;
                    ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "Name");
                    return View(new Jobs());
                }
            
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
                    Job.CreateDate = job.CreateDate;
                    //Job.subCategory = _context.SubCategories.FirstOrDefault(s => s.SubCategoryId == job.subCategoryId);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(PostJobSkills));
                }
                else if (HttpContext.Session.GetString("reuseJob") != null) {

                    var reusejobId = int.Parse(HttpContext.Session.GetString("reuseJob"));
                    var OldJob = _context.Jobs.FirstOrDefault(s => s.Id == reusejobId);
                    Jobs reuseJob = new Jobs();
                    reuseJob.Type = OldJob.Type;
                    reuseJob.Scope = OldJob.Scope;
                    reuseJob.Duration = OldJob.Duration;
                    reuseJob.LevelOfExperience = OldJob.LevelOfExperience;
                    reuseJob.TypeOfBudget = OldJob.TypeOfBudget;
                    reuseJob.BudgetFrom = OldJob.BudgetFrom;
                    reuseJob.BudgetTo = OldJob.BudgetTo;
                    reuseJob.JobDescription = OldJob.JobDescription;
                    reuseJob.Language_ProficiencyId = OldJob.Language_ProficiencyId;
                    reuseJob.TimeRequirement = OldJob.TimeRequirement;
                    reuseJob.TalentType = OldJob.TalentType;
                    _context.Add(reuseJob);
                    await _context.SaveChangesAsync();
                    List<JobsSkills> skills = new List<JobsSkills>(_context.JobsSkills.Where(a => a.JobsId == OldJob.Id));
                    foreach (var item in skills)
                    {
                        JobsSkills skill = new JobsSkills() { JobsId = reuseJob.Id, skillId = item.skillId };
                        _context.JobsSkills.Add(skill);
                        await _context.SaveChangesAsync();
                    }
                    reuseJob.Title = job.Title;
                    reuseJob.subCategoryId = job.subCategoryId;
                    reuseJob.CreateDate = job.CreateDate;
                    reuseJob.IsDraft = true;
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("JobId", reuseJob.Id.ToString());
                    HttpContext.Session.Remove("reuseJob");
                    return RedirectToAction(nameof(PostJobSkills));
                }
                else
                {
                    _context.Add(job);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("JobId", job.Id.ToString());
                    var Job = _context.Jobs.FirstOrDefault(s => s.Id == job.Id);
                    Job.Type = HttpContext.Session.GetString("JobType");
                    await _context.SaveChangesAsync();
                    HttpContext.Session.Remove("JobType");
                    return RedirectToAction(nameof(PostJobSkills));
                }
            }
            return View(job);
        }
        public IActionResult PostJobSkills()
        {
            if (HttpContext.Session.GetString("JobId") != null)
            {
                var jobId = int.Parse(HttpContext.Session.GetString("JobId"));
                var jobOld = _context.Jobs.FirstOrDefault(a => a.Id == jobId);
                List<JobsSkills> SkillsList = new List<JobsSkills>();
                SkillsList.AddRange(_context.JobsSkills.Where(a => a.JobsId == jobId));
                List<Skill> Skills = new List<Skill>();
                Skills.AddRange(_context.Skills.Where(a => a.SubCategoryId == jobOld.subCategoryId));
                //var Skills = _context.Skills.Where(a => a.SubCategoryId == jobOld.subCategoryId);
                //ViewData["jobId"] = jobId;
                //return View(jobOld);
                if (SkillsList.Count() > 0)
                {
                    //List<Skill> unselected = new List<Skill>();
                    List<Skill> selectedSkills = new List<Skill>();
                    for (var s=0; s<Skills.Count(); s++)
                    {
                        if (SkillsList.Where(a => a.skillId == Skills[s].SkillId).Count() == 1)
                        { 
                            selectedSkills.Add(Skills[s]);
                        }
                    }
                    Skills.RemoveAll(x => selectedSkills.Contains(x));
                    ViewData["SkillsSelected"] = selectedSkills;
                    ViewData["skillsSelectedCount"] = selectedSkills.Count();
                    ViewData["Skills"] = Skills;
                    ViewData["jobId"] = jobId;
                    return View(jobOld);
                }
                else
                {
                    ViewData["Skills"] = Skills;
                    ViewData["jobId"] = jobId;
                    return View(jobOld);
                }
            }
            else
            {
                return RedirectToAction(nameof(PostJobTitle));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJobSkills( IFormCollection job)
        {
            //return Json(Request.Form["Skills"]);
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("JobId") != null)
                {
                    var jobId = int.Parse(HttpContext.Session.GetString("JobId"));
                    var Job = _context.Jobs.FirstOrDefault(s => s.Id == jobId);
                    if (_context.JobsSkills.Where(a => a.JobsId == jobId).Count() > 0)
                    {
                        foreach(var item in _context.JobsSkills.Where(a => a.JobsId == jobId))
                        {
                            _context.JobsSkills.Remove(item);
                        }
                      await  _context.SaveChangesAsync();
                    }
                    foreach (var item in Request.Form["Skills"])
                    {
                            JobsSkills skill = new JobsSkills() { JobsId = jobId, skillId = int.Parse(item) };
                            _context.JobsSkills.Add(skill);
                            await _context.SaveChangesAsync();
                    }
                    return RedirectToAction(nameof(PostJobScope));
                }
                else
                {
                    return RedirectToAction(nameof(PostJobTitle));
                }
            }
            return View(job);
        }
        /*public async Task<IActionResult> GetSkills(int Id)
        {
            var SkillsList = _context.Skills.Where(a => a.SubCategoryId == Id);
            return Json(SkillsList);
        }*/
        public IActionResult PostJobScope()
        {
            if (HttpContext.Session.GetString("JobId") != null)
            {
                var jobId = int.Parse(HttpContext.Session.GetString("JobId"));
                var jobOld = _context.Jobs.FirstOrDefault(a => a.Id == jobId);
                return View(jobOld);
            }
            else
            {
                return RedirectToAction(nameof(PostJobSkills));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJobScope(Jobs job)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("JobId") != null)
                {
                    var jobId = int.Parse(HttpContext.Session.GetString("JobId"));
                    var Job = _context.Jobs.FirstOrDefault(s => s.Id == jobId);
                    Job.Scope = job.Scope;
                    Job.Duration = job.Duration;
                    Job.LevelOfExperience = job.LevelOfExperience;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(PostJobBudget));
                }
                else
                {
                    return RedirectToAction(nameof(PostJobSkills));
                }
            }
            return View(job);
        }
        public IActionResult PostJobBudget()
        {
            if (HttpContext.Session.GetString("JobId") != null)
            {
                var jobId = int.Parse(HttpContext.Session.GetString("JobId"));
                var jobOld = _context.Jobs.FirstOrDefault(a => a.Id == jobId);
                return View(jobOld);
            }
            else
            {
                return RedirectToAction(nameof(PostJobSkills));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJobBudget(Jobs job)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("JobId") != null)
                {
                    var jobId = int.Parse(HttpContext.Session.GetString("JobId"));
                    var Job = _context.Jobs.FirstOrDefault(s => s.Id == jobId);
                    Job.TypeOfBudget = job.TypeOfBudget;
                    if (job.TypeOfBudget == true)
                    {
                        Job.BudgetTo = Job.BudgetFrom = job.BudgetFrom;
                    }
                    else
                    {
                        Job.BudgetFrom = job.BudgetFrom;
                        Job.BudgetTo =job.BudgetTo ;
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ReviewJobPosting));
                }
                else
                {
                    return RedirectToAction(nameof(PostJobSkills));
                }
            }
            return View(job);
        }
        public IActionResult ReviewJobPosting()
        {

            if (HttpContext.Session.GetString("JobId") != null)
            {
                var jobId = int.Parse(HttpContext.Session.GetString("JobId"));
                var jobOld = _context.Jobs.FirstOrDefault(a => a.Id == jobId);
                var subCategoryName = (_context.SubCategories.FirstOrDefault(a => a.SubCategoryId == jobOld.subCategoryId)).Name;
                ViewData["JobSkills"] = _context.JobsSkills.Where(a => a.JobsId == jobId);
                ViewData["Skills"] = _context.Skills;
                ViewData["subcategName"] = subCategoryName;
                ViewData["LanguageLevel"] = _context.Language_Proficiency;
                //return Json(jobOld);
                return View(jobOld);
            }
            else
            {
                return RedirectToAction(nameof(PostJobBudget));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewJobPosting(Jobs job)
        {
            //if (ModelState.IsValid)
            //{
                if (HttpContext.Session.GetString("JobId") != null)
                {
                    var jobId = int.Parse(HttpContext.Session.GetString("JobId"));
                    var Job = _context.Jobs.FirstOrDefault(s => s.Id == jobId);
                    Job.Title = job.Title;
                    Job.JobDescription = job.JobDescription;
                    Job.Language_ProficiencyId = job.Language_ProficiencyId;
                    Job.TimeRequirement = job.TimeRequirement;
                    Job.TalentType = job.TalentType;
                    Job.IsDraft = job.IsDraft;
                    await _context.SaveChangesAsync();
                    HttpContext.Session.Remove("JobId");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction(nameof(PostJobBudget));
                }
            //}
            //return View(job);
        }
        public async Task<IActionResult> DeleteJob(int id)
        {
            if (_context.JobsSkills.Where(a => a.JobsId == id).Count() > 0)
            {
                foreach (var item in _context.JobsSkills.Where(a => a.JobsId == id))
                {
                    _context.JobsSkills.Remove(item);
                }
                await _context.SaveChangesAsync();
            }
            //Jobs deletedJob = new Jobs();
            var deletedJob = _context.Jobs.FirstOrDefault(a => a.Id == id);
            _context.Jobs.Remove(deletedJob);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult EditJob(int id)
        {
            var editJob = _context.Jobs.FirstOrDefault(a => a.Id == id);
            if (editJob != null)
            {
                HttpContext.Session.SetString("JobId", editJob.Id.ToString());
                if (_context.JobsSkills.Where(a => a.JobsId == editJob.Id).Count() < 1)
                {
                    return RedirectToAction(nameof(PostJobSkills));
                }else if(editJob.Scope == null)
                {
                    return RedirectToAction(nameof(PostJobScope));
                }else if(editJob.TypeOfBudget == null)
                {
                    return RedirectToAction(nameof(PostJobBudget));
                }
                else
                {
                    return RedirectToAction(nameof(ReviewJobPosting));
                }
            }
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult AllJobPosts(string drafted=null)
        {
            if (drafted != null)
            {
                if (drafted == "true")
                {
                    List<Jobs> allJobsdrafted = new List<Jobs>(_context.Jobs.Where(a=>a.IsDraft==true));
                    allJobsdrafted.Reverse();
                    return View(allJobsdrafted);
                }else
                {
                    List<Jobs> allJobsposted = new List<Jobs>(_context.Jobs.Where(a => a.IsDraft == false));
                    allJobsposted.Reverse();
                    return View(allJobsposted);
                }
            }
            else
            {
                List<Jobs> allJobs = new List<Jobs>(_context.Jobs);
                allJobs.Reverse();
                return View(allJobs);
            }
            
        }
        public IActionResult JobDetails(int id)
        {

            return View();
        }
        public  IActionResult ProjectsCatalog()
        {
            var Projects = _context.Projects.Include(a => a.client_Projects).Include(a => a.Freelancer.User).Include(a=>a.SubCategory).ToList();
            return View(Projects);
        }
        public async Task<IActionResult> SaveProject(int id)
        {
            var ClientId = "a123";
            Client_Projects Projected =  _context.Client_Projects.Where(a => a.ProjectId == id && a.ClientId == ClientId).FirstOrDefault();
            if (Projected == null)
            {
                Client_Projects client_Project = new Client_Projects() { ClientId = ClientId, ProjectId = id, IsSaved = true };
                _context.Client_Projects.Add(client_Project);
                _context.SaveChanges();
            }
            else
            {
                if (Projected.IsSaved == false)
                {
                    Projected.IsSaved = true;
                    _context.SaveChanges();
                }
            }

               return Ok();

            }

        public async Task<IActionResult> UnSaveProjecte(int id)
        {
            var ClientId = "a123";
            Client_Projects Projected = _context.Client_Projects.Where(a => a.ProjectId == id && a.ClientId == ClientId).FirstOrDefault();
            if (Projected != null)
            {
                Projected.IsSaved = false;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }


    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Data;
using Upwork.Models;
using Upwork.Models.DbModels;
using Upwork.Models.ViewModels;

namespace Upwork.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private readonly IHostingEnvironment _hostenviroment;
        public ClientController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment, SignInManager<ApplicationUser> _signInManager,
            UserManager<ApplicationUser> _userManager,
            RoleManager<IdentityRole> _roleManager)
        {
            _context = context;
            signInManager = _signInManager;
            userManager = _userManager;
            roleManager = _roleManager;
            _hostenviroment = hostingEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var u = await userManager.GetUserAsync(User);
            ViewData["username"] = u.UserName;
            Client client = _context.Clients.FirstOrDefault(a => a.ClientId == u.Id);
            
            ViewData["ClientName"] = (u.FirstName + " " + u.LastName);
            List<Jobs> alljobs = new List<Jobs>(_context.Jobs.Include(a=>a.freelancer_Jobs).Where(j=>j.ClientId==client.ClientId));
            return View(alljobs);
        }
        public IActionResult PostJob(int id)
        {
            if (id != 0)
            {
                var Job = _context.Jobs.FirstOrDefault(a => a.Id == id);
                return View(Job);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJob(Jobs job)
        {
            var u = await userManager.GetUserAsync(User);
            ViewData["username"] = u.UserName;
            if (job != null)
            {
                var jobOld = _context.Jobs.FirstOrDefault(a => a.Id == job.Id);
                if (jobOld != null)
                {
                    jobOld.Type = job.Type;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("PostJobTitle", new { id = jobOld.Id });
                }
                else
                {
                    HttpContext.Session.SetString("JobType", job.Type.ToString());
                    return RedirectToAction("PostJobTitle");
                }
            }
            return View(job);
        }

        public async Task<IActionResult> PostJobTitle(int id)
        {
            var u = await userManager.GetUserAsync(User);
            ViewData["username"] = u.UserName;
            if (id != 0)
            {
                var jobOld = _context.Jobs.FirstOrDefault(a => a.Id == id);
                if (jobOld != null)
                {
                    if (jobOld.subCategoryId != null)
                    {
                        var categoryId = (_context.SubCategories.FirstOrDefault(a => a.SubCategoryId == jobOld.subCategoryId)).CategoryId;
                        ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "Name", categoryId);
                        return View(jobOld);
                    }
                    else
                    {
                        ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "Name");
                        return View(jobOld);
                    }
                }
                else
                {
                    ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "Name");
                    return View(new Jobs());
                }
            }
            else
            {
                ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "Name");
                return View(new Jobs());
            }

        }
        public async Task<IActionResult> GetSubCategories(int Id)
        {
            //var jobs = _context.JobsSkills.Include(a => a.Jobs).Include(b => b.skill).ToList();
            //return View(jobs);
            var SubCategoryList = _context.SubCategories.Where(a => a.CategoryId == Id);
            return Json(SubCategoryList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJobTitle(Jobs job)
        {
            var u = await userManager.GetUserAsync(User);
            ViewData["username"] = u.UserName;
            if (job != null)
            {
                var Job = _context.Jobs.FirstOrDefault(s => s.Id == job.Id);
                if (Job != null && Job.IsDraft == false)
                {

                    Jobs reuseJob = new Jobs();
                    reuseJob.Type = Job.Type;
                    reuseJob.Scope = Job.Scope;
                    reuseJob.Duration = Job.Duration;
                    reuseJob.LevelOfExperience = Job.LevelOfExperience;
                    reuseJob.TypeOfBudget = Job.TypeOfBudget;
                    reuseJob.BudgetFrom = Job.BudgetFrom;
                    reuseJob.BudgetTo = Job.BudgetTo;
                    reuseJob.JobDescription = Job.JobDescription;
                    reuseJob.Language_ProficiencyId = Job.Language_ProficiencyId;
                    reuseJob.TimeRequirement = Job.TimeRequirement;
                    reuseJob.TalentType = Job.TalentType;
                    reuseJob.ClientId = u.Id;
                    reuseJob.CreateDate = DateTime.Now;
                    reuseJob.DraftSavedDate = DateTime.Now;
                    _context.Add(reuseJob);
                    await _context.SaveChangesAsync();
                    List<JobsSkills> skills = new List<JobsSkills>(_context.JobsSkills.Where(a => a.JobsId == Job.Id));
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
                    return RedirectToAction("PostJobSkills", new { id = reuseJob.Id });
                }
                else if (Job != null)
                {
                    Job.Title = job.Title;
                    Job.subCategoryId = job.subCategoryId;
                    Job.DraftSavedDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("PostJobSkills", new { id = Job.Id });
                }
                else
                {
                    var newJob = job;
                    _context.Add(newJob);
                    await _context.SaveChangesAsync();
                    newJob.Type = HttpContext.Session.GetString("JobType");
                    newJob.ClientId = u.Id;
                    newJob.CreateDate = DateTime.Now;
                    newJob.DraftSavedDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    HttpContext.Session.Remove("JobType");
                    return RedirectToAction("PostJobSkills", new { id = newJob.Id });
                }
            }
            return View(job);
        }
        public async Task<IActionResult> PostJobSkills(int id)
        {
            var u = await userManager.GetUserAsync(User);
            ViewData["username"] = u.UserName;
            if (id != 0)
            {
                var jobOld = _context.Jobs.FirstOrDefault(a => a.Id == id);
                if (jobOld != null)
                {
                    List<JobsSkills> SkillsList = new List<JobsSkills>();
                    SkillsList.AddRange(_context.JobsSkills.Where(a => a.JobsId == jobOld.Id));
                    List<Skill> Skills = new List<Skill>();
                    Skills.AddRange(_context.Skills.Where(a => a.SubCategoryId == jobOld.subCategoryId));
                    if (SkillsList.Count() > 0)
                    {
                        List<Skill> selectedSkills = new List<Skill>();
                        for (var s = 0; s < Skills.Count(); s++)
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
                        ViewData["jobId"] = jobOld.Id;
                        return View(jobOld);
                    }
                    else
                    {
                        ViewData["Skills"] = Skills;
                        ViewData["jobId"] = jobOld.Id;
                        return View(jobOld);
                    }
                }
                else
                {
                    return RedirectToAction("PostJobTitle");
                }
            }
            else
            {
                return RedirectToAction("PostJobTitle");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJobSkills(IFormCollection job, Jobs j)
        {
                if (j != null)
                {
                    var Job = _context.Jobs.FirstOrDefault(s => s.Id == j.Id);
                    if (_context.JobsSkills.Where(a => a.JobsId == j.Id).Count() > 0)
                    {
                        foreach (var item in _context.JobsSkills.Where(a => a.JobsId == j.Id))
                        {
                            _context.JobsSkills.Remove(item);
                        }
                        await _context.SaveChangesAsync();
                    }
                    foreach (var item in Request.Form["Skills"])
                    {
                        JobsSkills skill = new JobsSkills() { JobsId = j.Id, skillId = int.Parse(item) };
                        _context.JobsSkills.Add(skill);
                        await _context.SaveChangesAsync();
                    }
                    Job.DraftSavedDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("PostJobScope", new { id = j.Id });
                }
                else
                {
                    return View(job);
                }
        }
        public async Task<IActionResult> PostJobScope(int id)
        {
            var u = await userManager.GetUserAsync(User);
            ViewData["username"] = u.UserName;
            if (id != 0)
            {
                var jobOld = _context.Jobs.FirstOrDefault(a => a.Id == id);
                if (jobOld != null)
                {
                    return View(jobOld);
                }
                else
                {
                    return RedirectToAction("PostJobSkills");
                }
            }
            else
            {
                return RedirectToAction("PostJobSkills");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJobScope(Jobs job)
        {
                if (job != null)
                {
                    var Job = _context.Jobs.FirstOrDefault(s => s.Id == job.Id);
                    if(job != null)
                    {
                        Job.Scope = job.Scope;
                        Job.Duration = job.Duration;
                        Job.LevelOfExperience = job.LevelOfExperience;
                        Job.DraftSavedDate = DateTime.Now;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("PostJobBudget", new { id = Job.Id });
                    }
                    else
                    {
                        return RedirectToAction("PostJobSkills");
                    }

            }
            return View(job);

        }
        public async Task<IActionResult> PostJobBudget(int id)
        {
            var u = await userManager.GetUserAsync(User);
            ViewData["username"] = u.UserName;
            if (id != 0)
            {
                var jobOld = _context.Jobs.FirstOrDefault(a => a.Id == id);
                if (jobOld != null)
                {
                    return View(jobOld);
                }
                else
                {
                    return RedirectToAction("PostJobSkills");
                }
            }
            else
            {
                return RedirectToAction("PostJobSkills");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJobBudget(Jobs job)
        {
                if (job != null)
                {
                    var Job = _context.Jobs.FirstOrDefault(s => s.Id == job.Id);
                    if(Job != null)
                    {
                        Job.TypeOfBudget = job.TypeOfBudget;
                        if (job.TypeOfBudget == true)
                        {
                            Job.BudgetTo = Job.BudgetFrom = job.BudgetFrom;
                        }
                        else
                        {
                            Job.BudgetFrom = job.BudgetFrom;
                            Job.BudgetTo = job.BudgetTo;
                        }
                        Job.DraftSavedDate = DateTime.Now;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("ReviewJobPosting", new { id = Job.Id });
                    }
                    else
                    {
                        return RedirectToAction("PostJobSkills");
                    }

            }
            return View(job);

        }
        public async Task<IActionResult> ReviewJobPosting(int id)
        {
            var u = await userManager.GetUserAsync(User);
            ViewData["username"] = u.UserName;
            if (id != 0)
            {
                var jobOld = _context.Jobs.FirstOrDefault(a => a.Id == id);
                if (jobOld != null)
                {
                    var subCategoryName = (_context.SubCategories.FirstOrDefault(a => a.SubCategoryId == jobOld.subCategoryId)).Name;
                    ViewData["JobSkills"] = _context.JobsSkills.Where(a => a.JobsId == jobOld.Id);
                    ViewData["Skills"] = _context.Skills;
                    ViewData["subcategName"] = subCategoryName;
                    ViewData["LanguageLevel"] = _context.Language_Proficiency;
                    ViewData["Questions"] = _context.ReviewJobQuestions;
                    //return Json(jobOld);
                    return View(jobOld);
                }
                else
                {
                    return RedirectToAction("PostJobBudget");
                }
            }
            else
            {
                return RedirectToAction("PostJobBudget");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewJobPosting(Jobs job)
        {
            //if (ModelState.IsValid)
            //{
                if (job != null)
                {
                    var Job = _context.Jobs.FirstOrDefault(s => s.Id == job.Id);
                    if (Job != null)
                    {
                        Job.Title = job.Title;
                        Job.JobDescription = job.JobDescription;
                        Job.Language_ProficiencyId = job.Language_ProficiencyId;
                        Job.TimeRequirement = job.TimeRequirement;
                        Job.TalentType = job.TalentType;
                        Job.IsDraft = job.IsDraft;
                        Job.DraftSavedDate = DateTime.Now;
                        await _context.SaveChangesAsync();
                    if (Job.IsDraft == false)
                    {
                        Job.CreateDate = DateTime.Now;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("JobDetails", new { id = Job.Id });
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return RedirectToAction("PostJobBudget");
            //}
            //return View(job);
        }
        
        public async Task<IActionResult> CloseJop(int id)
        {
            var CloseJob = _context.Jobs.FirstOrDefault(a => a.Id == id);
            CloseJob.IsCanceled = true;
            CloseJob.JobClosedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
                if (_context.JobsSkills.Where(a => a.JobsId == editJob.Id).Count() < 1)
                {
                    return RedirectToAction("PostJobSkills", new { id = id });
                }
                else if (editJob.Scope == null)
                {
                    return RedirectToAction("PostJobScope", new { id = id });
                }
                else if (editJob.TypeOfBudget == null)
                {
                    return RedirectToAction("PostJobBudget", new { id = id });
                }
                else
                {
                    return RedirectToAction("ReviewJobPosting", new { id = id });
                }
            }
            return View();
        }
        public async Task<IActionResult> Profile()
        {
            var u = await userManager.GetUserAsync(User);
            ViewData["username"] = u.UserName;
            Client client = _context.Clients.Include(a => a.User).ThenInclude(a => a.Country).Where(a => a.ClientId == u.Id).FirstOrDefault();
            ViewData["Countries"] = new SelectList(_context.Countries, "CountryId", "Name", client.User.CountryId);
            return View(client);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ApplicationUser data)
        {
            var u = await userManager.GetUserAsync(User);
            Client client = _context.Clients.Include(a => a.User).ThenInclude(c => c.Country).Where(a => a.ClientId == u.Id).FirstOrDefault();
            if (ModelState.IsValid)
            {
                client.User.UserName = data.UserName;
                client.User.FirstName = data.FirstName;
                client.User.LastName = data.LastName;
                client.User.Email = data.Email;
                client.User.CountryId = data.CountryId;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Profile));
            }
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePass(string ConfirmPassword, string OldPassword)
        {
            var u = await userManager.GetUserAsync(User);
            Client client = _context.Clients.Include(a => a.User).ThenInclude(c => c.Country).Where(a => a.ClientId == u.Id).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (await userManager.CheckPasswordAsync(client.User, OldPassword) == true)
                {
                    var result = await userManager.ChangePasswordAsync(u, OldPassword, ConfirmPassword);
                    //client.User.PasswordHash = ConfirmPassword;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Profile));
                }

            }
            return RedirectToAction(nameof(Profile));
        }
        public async Task<IActionResult> AllJobPosts(string drafted = null)
        {
            var u = await userManager.GetUserAsync(User);
            ViewData["username"] = u.UserName;
            if (drafted != null)
            {
                if (drafted == "true")
                {
                    List<Jobs> allJobsdrafted = new List<Jobs>(_context.Jobs.Where(a => a.IsDraft == true));
                    allJobsdrafted.Reverse();
                    return View(allJobsdrafted);
                }
                else if (drafted == "false")
                {
                    List<Jobs> allJobsposted = new List<Jobs>(_context.Jobs.Where(a => a.IsDraft == false));
                    allJobsposted.Reverse();
                    return View(allJobsposted);
                }
                return Ok();
            }
            else
            {
                List<Jobs> allJobs = new List<Jobs>(_context.Jobs.Include(a=>a.freelancer_Jobs).Where(a=>a.ClientId==u.Id));
                allJobs.Reverse();
                return View(allJobs);
            }

        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> JobDetails(int id)
        {
            var CurrentUser = await userManager.GetUserAsync(User);
            ViewData["username"] = CurrentUser.UserName;
            var job = _context.Jobs.Include(a => a.subCategory).FirstOrDefault(a => a.Id == id && a.ClientId == CurrentUser.Id);
            if(job == null)
            {
                return NotFound();
            }

            List<JobsSkills> SkillsList = new List<JobsSkills>();
            SkillsList.AddRange(_context.JobsSkills.Where(a => a.JobsId == id));
            List<Skill> Skills = new List<Skill>();
            Skills.AddRange(_context.Skills.Where(a => a.SubCategoryId == job.subCategoryId));
            if (SkillsList.Count() > 0)
            {
                List<Skill> selectedSkills = new List<Skill>();
                for (var s = 0; s < Skills.Count(); s++)
                {
                    if (SkillsList.Where(a => a.skillId == Skills[s].SkillId).Count() == 1)
                    {
                        selectedSkills.Add(Skills[s]);
                    }
                }
                ViewData["jobSkills"] = selectedSkills;
                
            }

            ViewData["User"] = _context.Freelancers.Include(a => a.Freelancer_Jobs).Include(a => a.User).Include(a => a.City).Include(a => a.Skills).Include(a => a.Languages);
            ViewData["FreelancerSkills"] = _context.Freelancer_Skill.Include(a => a.Skill);

            return View(job);
        }

        public async Task<IActionResult> GetProposal(int id)
        {
            var ProPosals = _context.Freelancer_Jobs.Where(a => a.JobsId == id && a.IsProposal == true).Include(a => a.Freelancer).ToList();
            ViewData["User"] = _context.Freelancers.Include(a => a.Freelancer_Jobs).Include(a => a.User).Include(a => a.City).Include(a => a.Skills).Include(a => a.Languages);
            return PartialView(ProPosals);
        }
        public async Task<IActionResult> GetHired(int id)
        {
            var Hired = _context.Freelancer_Jobs.Where(a => a.JobsId == id && a.IsHire == true).Include(a => a.Freelancer).ToList();
            ViewData["User"] = _context.Freelancers.Include(a => a.Freelancer_Jobs).Include(a => a.User).Include(a => a.City).Include(a => a.Skills).Include(a => a.Languages);
            return PartialView(Hired);
        }

        [HttpPost]
        public async Task<ActionResult> Hire(string FreelancerId , int JobsId, IFormFile Contract)
        {
            if (ModelState.IsValid)
            {
                var Freelancer = _context.Freelancers.Include(a => a.User).FirstOrDefault(a => a.FreelancerId == FreelancerId);
                var Job = _context.Jobs.Include(a => a.Client).FirstOrDefault(a => a.Id == JobsId);
                var FreelancerJob = _context.Freelancer_Jobs.FirstOrDefault(a => a.JobsId == JobsId && a.FreelancerId == FreelancerId && a.IsProposal == true);
                string ContractName = null;
                if (Contract != null)
                {
                    string folder = Path.Combine(_hostenviroment.WebRootPath, "contracts");
                    ContractName = Freelancer.User.UserName + "_" + Job.Title+ Path.GetExtension(Contract.FileName);
                    string photoPath = Path.Combine(folder, ContractName);
                    Contract.CopyTo(new FileStream(photoPath, FileMode.Create));
                }
                FreelancerJob.IsHire = true;
                FreelancerJob.IsProposal = false;
                FreelancerJob.Contract = ContractName;
                _context.SaveChanges();
                return Ok();
            }
            
            return BadRequest();
        }


        public IActionResult ProjectsCatalog()
        {
            var Projects = _context.Projects.Include(a => a.client_Projects).Include(a => a.Freelancer.User).Include(a => a.SubCategory).ToList();
            return View(Projects);
        }


        public async Task<IActionResult> JobRate(int? Id, int? Rate, string FreelancerId)
        {
            if (Id == null || Rate == null || FreelancerId == null)
            {
                return BadRequest();
            }
            var Job = _context.Jobs.Where(a => a.Id == Id).FirstOrDefault();
            if (Job == null)
            {
                return NotFound();
            }

            return Ok();
        }

        public async Task<IActionResult> SaveProject(int id)
        {
            var ClientId = "a123";
            Client_Projects Projected = _context.Client_Projects.Where(a => a.ProjectId == id && a.ClientId == ClientId).FirstOrDefault();
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
        public async Task<IActionResult> SearchForProject(string search)
        {
            var Freelancer = "a123";
            if (string.IsNullOrEmpty(search))
            {
                return BadRequest();
            }
            var Skill = _context.Skills.FirstOrDefault(s => s.Name == search);
            var ProjectList = new List<Project>();
            if (Skill != null)
            {
                var SearchBySkill = _context.ProjectSkills.Where(a => a.SkillId == Skill.SkillId).ToList();
                foreach (var item in SearchBySkill)
                {
                    var p = _context.Projects.Include(a => a.Skills).Include(a => a.client_Projects).Include(a => a.SubCategory).Include(a => a.Freelancer.User).FirstOrDefault(a => a.ProjectId == item.ProjectId);
                    ProjectList.Add(p);
                }
            }
            var project = _context.Projects.Where(a => a.Title.Contains(search) || a.Description.Contains(search)).Include(a => a.Freelancer.User).Include(a => a.Skills).Include(a => a.client_Projects).Include(a => a.SubCategory).ToList();
            if (project == null && (ProjectList.Count() == 0))
            {
                return NotFound();
            }
            ViewData["Skills"] = _context.ProjectSkills.Select(s => s.Skill).ToList();
            return View("ProjectsCatalog", project.Union(ProjectList));
        }

        public IActionResult ProjectDetails(int id)
        {
           
            var Project = _context.Projects.Include(a => a.Skills).Include(a => a.SubCategory).
                Include(a => a.Freelancer).Include(a => a.Freelancer.Experiences).Include(a => a.Freelancer.User).Include(a => a.Freelancer.City).FirstOrDefault(a => a.ProjectId == id);
            ViewData["Experience"] = _context.Freelancer_Experience.Where(a => a.FreelancerId == Project.FreelancerId).Include(a => a.Company).Include(a => a.Country).Include(a => a.JobTitle).ToList();
            return View(Project);
        }

        public async Task<IActionResult> AllContractsAsync()
        {
            var u = await userManager.GetUserAsync(User);
            Client client = _context.Clients.Include(a => a.User).ThenInclude(c => c.Country).Where(a => a.ClientId == u.Id).FirstOrDefault();
            var clientContract = new List<Freelancer_Job>();
            foreach(var job in _context.Freelancer_Jobs.Include(a => a.Jobs).Include(a => a.Freelancer))
            {
                if(job.Jobs.ClientId == client.ClientId && job.IsHire == true)
                {
                    clientContract.Add(job);
                }
            }
            ViewData["Users"] = _context.Users;
            return View(clientContract);
        }
        public async Task<IActionResult> DeleteAccount(string id)
        {
            if(id != null)
            {
                var client = _context.Clients.FirstOrDefault(a => a.ClientId == id);
                _context.Remove(client);
                await _context.SaveChangesAsync();
                return RedirectToAction("index", "Home");
            }
            return View();
        }
        public async Task<IActionResult> MyHire()
        {
            var u = await userManager.GetUserAsync(User);
            var Hired = _context.Freelancer_Jobs.Include(a=>a.Jobs).Where(a => a.Jobs.ClientId == u.Id && a.IsHire == true).Include(a => a.Freelancer).ToList();
            ViewData["User"] = _context.Freelancers.Include(a => a.Freelancer_Jobs).Include(a => a.User).Include(a => a.City).Include(a => a.Skills).Include(a => a.Languages);

            return View(Hired);
        }
        //[HttpPost]
        //public async Task<IActionResult> Rate(Freelancer_Job job)
        //{
        //    var freelancerJob = _context.Freelancer_Jobs.FirstOrDefault(a => a.JobsId == job.JobsId && a.FreelancerId == job.FreelancerId);
        //    freelancerJob.Rate = job.Rate;
        //    await _context.SaveChangesAsync();
        //    return View();
        //}

    }
    }

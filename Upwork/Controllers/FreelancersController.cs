using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Upwork.Data;
using Upwork.Models;
using Upwork.Models.DbModels;
using Upwork.Models.ViewModels.Register;

namespace Upwork.Controllers
{
    [Authorize(Roles = "Freelancer")]
    public class FreelancersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly IWebHostEnvironment hosting;
        private SignInManager<ApplicationUser> signInManager;

        public FreelancersController(ApplicationDbContext context , UserManager<ApplicationUser> usermanager, IWebHostEnvironment _hosting, SignInManager<ApplicationUser> _signInManager)
        {
            _context = context;
            _UserManager = usermanager;
            hosting = _hosting;
            signInManager = _signInManager;
        }

        [Authorize(Roles = "Freelancer")]
        // GET: Freelancers
        public async Task<IActionResult> Index()
        {
            var CurrentUser = await _UserManager.GetUserAsync(User);
            var freelancer = await _context.Freelancers.Include(a=>a.SubCategory).Include(a=>a.Category).Include(a => a.Freelancer_Jobs).Include(a =>a.User).FirstOrDefaultAsync(a =>a.FreelancerId == CurrentUser.Id);
            if(freelancer == null)
            {
                return BadRequest();
            }
            var Jobs = await _context.Jobs.Where(a => a.IsDraft == false && a.IsCanceled == false).Include(a=>a.jobsSkills).Include(a => a.freelancer_Jobs).ToListAsync();

            var Dislikejobs =  _context.Freelancer_Jobs.Where(a => a.Isdislike == true &&a.FreelancerId==CurrentUser.Id).Select(a => a.Jobs).ToList();
            var jobskills = _context.JobsSkills.Select(s => s.skill);
            ViewData["Skills"] = jobskills.ToList();
            ViewData["Freelancer"] = freelancer;
            return View(Jobs.Except(Dislikejobs));
        }

        // GET: Freelancers/Profile/5
        public async Task<IActionResult> Profile()
        {
            var CurrentUser = await _UserManager.GetUserAsync(User);
            var freelancer = await _context.Freelancers.Include(a => a.Educations).Include(a => a.SubCategory).Include(a => a.Category).Include(a => a.Freelancer_Jobs).Include(a => a.User).Include(a => a.City).Include(a => a.Skills).Include(a => a.Languages).FirstOrDefaultAsync(a => a.FreelancerId == CurrentUser.Id);
            
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
        //Post: Freelancer/SearchForJob
        [HttpPost]
        public async Task<IActionResult> SearchForJob(string search)
        {
            var CurrentUser = await _UserManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(search))
            {
                return BadRequest();
            }
            var Skill = _context.Skills.FirstOrDefault(s => s.Name == search);
            var JobList = new List<Jobs>();
            if(Skill != null)
            {
                var SearchBySkill = _context.JobsSkills.Where(a => a.skillId == Skill.SkillId).ToList();
                foreach(var item in SearchBySkill)
                {
                    var j = _context.Jobs.Include(a => a.jobsSkills).Include(a => a.freelancer_Jobs).FirstOrDefault(a => a.Id == item.JobsId);
                    JobList.Add(j);
                }
            }
            var Job = _context.Jobs.Where(a => a.Title.Contains(search) || a.JobDescription.Contains(search)).Include(a=>a.jobsSkills).Include(a => a.freelancer_Jobs).ToList();
            if(Job == null && (JobList.Count() == 0))
            {
                return NotFound();
            }
            var SavesJobs = _context.Freelancer_Jobs.Include(a => a.Jobs).Where(a => a.FreelancerId == CurrentUser.Id && a.IsSaved == true && a.Isdislike == false).ToList();


            ViewData["Skills"] = _context.JobsSkills.Select(s => s.skill).ToList();
            ViewData["SavesJobs"] = SavesJobs;
            ViewData["SavesJobsCount"] = SavesJobs.Count();
            return View(Job.Union(JobList));
        }
        //Get: Freelancer/SaveJob
        public async Task<IActionResult> SaveJob(int id)
        {
            var CurrentUser = await _UserManager.GetUserAsync(User);
            Freelancer_Job savedJobs = _context.Freelancer_Jobs.Where(a => a.JobsId == id && a.FreelancerId == CurrentUser.Id).FirstOrDefault();
            if (savedJobs == null)
            {
                savedJobs = new Freelancer_Job() { FreelancerId = CurrentUser.Id, JobsId = id ,IsSaved=true };
                _context.Freelancer_Jobs.Add(savedJobs);
                _context.SaveChanges();
            }
            else
            {
                if(savedJobs.IsSaved == false)
                {
                    savedJobs.IsSaved = true;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction(nameof(Index));
        }
        //Get: Freelancer/UnSaveJob
        public async Task<IActionResult> UnSaveJob(int id)
        {
            var CurrentUser = await _UserManager.GetUserAsync(User);
            Freelancer_Job savedJobs = _context.Freelancer_Jobs.Where(a => a.JobsId == id && a.FreelancerId == CurrentUser.Id &&a.IsSaved==true).FirstOrDefault();
            if (savedJobs != null)
            {
               savedJobs.IsSaved = false;
                _context.SaveChanges();
                
            }
            return RedirectToAction(nameof(Index));
        }
        //Get: Freelancer/DislikeJob
        public async Task<IActionResult> DislikeJob(int id)
        {
            var CurrentUser = await _UserManager.GetUserAsync(User);
            Freelancer_Job savedJobs = _context.Freelancer_Jobs.Where(a => a.JobsId == id && a.FreelancerId == CurrentUser.Id ).FirstOrDefault();
            if (savedJobs == null)
            {
                savedJobs = new Freelancer_Job() { FreelancerId = CurrentUser.Id, JobsId = id, Isdislike = true };
                _context.Freelancer_Jobs.Add(savedJobs);
                _context.SaveChanges();
            }
            else
            {
                if (savedJobs.Isdislike != true)
                {
                    savedJobs.Isdislike = true;
                    _context.SaveChanges();
                }
            }
            return Ok();
        }
        //Get: Freelancer/SubmitProposal
        public async Task<IActionResult> SubmitProposal(int Id)
        {

            var CurrentUser = await _UserManager.GetUserAsync(User);
            var FreelancerJobs = _context.Freelancer_Jobs.Where(a => a.FreelancerId == CurrentUser.Id && a.JobsId==Id).FirstOrDefault();
            if(FreelancerJobs == null)
            {
                Freelancer_Job job = new Freelancer_Job() { FreelancerId = CurrentUser.Id, JobsId = Id, IsProposal = true };
                _context.Freelancer_Jobs.Add(job);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (FreelancerJobs.IsProposal == false)
                {
                    FreelancerJobs.IsProposal = true;
                    _context.SaveChanges();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        //Get: Freelancer/Proposal
        public async Task<IActionResult> AllProposal()
        {
            var CurrentUser = await _UserManager.GetUserAsync(User);
            var AllProposal = _context.Freelancer_Jobs.Where(a => a.FreelancerId == CurrentUser.Id && a.IsProposal == true).Include(a=>a.Jobs).Include(a=>a.Jobs.jobsSkills);
            ViewData["Skills"] = _context.JobsSkills.Select(s => s.skill).ToList();
            return View(AllProposal);
        }
        private bool FreelancerExists(string id)
        {
            return _context.Freelancers.Any(e => e.FreelancerId == id);
        }

        
        public async Task<IActionResult> Contracts()
        {
            var user = await _UserManager.GetUserAsync(User);
            var Freelancer = _context.Freelancers.FirstOrDefault(a => a.FreelancerId == user.Id);
            return View(_context.Freelancer_Jobs.Where(a => a.FreelancerId == Freelancer.FreelancerId && a.IsHire == true).Include(a=>a.Jobs).Include(a=>a.Jobs.Client.User).ToList());
        }

        
        public async Task<IActionResult> DownloadCSV(string FileName)
        {
            if (FileName == null)
            {
                return BadRequest();
            }
            string FolderPath = Path.Combine(hosting.WebRootPath, "Contracts");
            string FilePath = Path.Combine(FolderPath, FileName);
            var memory = new MemoryStream();
            using(var stream = new FileStream(FilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/pdf", Path.GetFileName(FilePath));
        }

        public async Task<IActionResult> SearchContract(string term)
        {
            if (term == null)
            {
                return RedirectToAction("Contracts");
            }
            var user = await _UserManager.GetUserAsync(User);
            var Freelancer = _context.Freelancers.FirstOrDefault(a => a.FreelancerId == user.Id);
            var result = _context.Freelancer_Jobs.Where(a => a.FreelancerId == Freelancer.FreelancerId && a.IsHire == true).Include(a => a.Jobs).Include(a => a.Jobs.Client.User).Where(a=>a.Jobs.Title.Contains(term) || a.Jobs.Client.User.FirstName.Contains(term) || a.Jobs.Client.User.LastName.Contains(term)).ToList();
            return View("Contracts", result);
        }

        public async Task<IActionResult> JobDetails(int Id)
        {
            var CurrentUser = await _UserManager.GetUserAsync(User);
            var freelancer = await _context.Freelancers.FirstOrDefaultAsync(a => a.FreelancerId == CurrentUser.Id);

            var Job = _context.Jobs.Include(a => a.Client).Include(a => a.jobsSkills).Include(a => a.subCategory).Include(a => a.freelancer_Jobs).Include(a =>a.Client).FirstOrDefault(a => a.Id == Id && a.IsDraft == false && a.IsCanceled == false);
            if(Job == null)
            {
                return NotFound();
            }
            ViewData["Skills"] = _context.JobsSkills.Where(a => a.JobsId == Id).Select(a => a.skill);
            ViewData["FreelancerJob"] = _context.Freelancer_Jobs.FirstOrDefault(a => a.JobsId == Id &&  a.FreelancerId == freelancer.FreelancerId);
            return View(Job);
        }

        public async Task<IActionResult> Settings()
        {
            var user = await _UserManager.GetUserAsync(User);           
            SettingsViewModel model = new SettingsViewModel() { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName , Username = user.UserName};
            ViewBag.HasPassword =  await _UserManager.HasPasswordAsync(user);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            var user = await _UserManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {               
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                var token = await _UserManager.GenerateChangeEmailTokenAsync(user, model.Email);
                await _UserManager.ChangeEmailAsync(user, model.Email, token);
                await _context.SaveChangesAsync();
                return RedirectToAction("Settings");
            }
            ViewBag.HasPassword = await _UserManager.HasPasswordAsync(user);
            return View(model);
        }

       public IActionResult ChangePassword()
       {
            return PartialView("ChangePasswordModal");
       }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _UserManager.GetUserAsync(User);
                var result = await _UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    if (model.RequiredToSignin)
                    {
                        await signInManager.SignOutAsync();
                        return RedirectToAction("Login", "Account");
                    }
                    return RedirectToAction("Settings");
                }
            }
            return PartialView("ChangePasswordModal",model);
        }

        public IActionResult ChangeProfilePhoto()
        {
            return PartialView("ChangePhotoModal");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfilePhoto(ProfilePhotoViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.File != null)
                {
                    var u = await _UserManager.GetUserAsync(User);
                    var Freelancer = _context.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                    string Uploads = Path.Combine(hosting.WebRootPath, "ProfilePhotos");
                    string FileName = Freelancer.FreelancerId + "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length - 1];
                    string FullPath = Path.Combine(Uploads, FileName);
                    if (Freelancer.Image != null)
                    {
                        System.IO.File.Delete(FullPath);
                    }
                    model.File.CopyTo(new FileStream(FullPath, FileMode.Create));
                    Freelancer.Image = FileName;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Profile");
                }
            }
            return PartialView("ChangePhotoModal");
        }


        public async Task<IActionResult> EditLanguages()
        {
            var u = await _UserManager.GetUserAsync(User);
            var Freelancer = _context.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            var EnglishId = _context.Languages.FirstOrDefault(a => a.Name == "English").LanguageId;
            LanguagesViewModel model = new LanguagesViewModel();
            if (_context.Freelancer_Language.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId && a.LanguageId == EnglishId) != null)
            {
                model.ProficiencyId = _context.Freelancer_Language.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId && a.LanguageId == EnglishId).ProficiencyId;
            }
            if (_context.Freelancer_Language.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId && a.LanguageId != EnglishId) != null)
            {
                var SecondLanguage = _context.Freelancer_Language.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId && a.LanguageId != EnglishId);
                model.Language1Id = SecondLanguage.LanguageId;
                model.Proficiency1Id = SecondLanguage.ProficiencyId;
            }
            ViewBag.ProficiencyId = new SelectList(_context.Language_Proficiency, "ProficiencyId", "Name");
            ViewBag.LanguageId = new SelectList(_context.Languages.Where(a => a.LanguageId != EnglishId), "LanguageId", "Name");
            ViewBag.SecondLanguage = _context.Languages.FirstOrDefault(a => a.LanguageId == model.Language1Id).Name;
            return PartialView("EditLanguages",model);
        }

        [HttpPost]
        public async Task<IActionResult> EditLanguages(LanguagesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await _UserManager.GetUserAsync(User);
                var Freelancer = _context.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                var EnglishId = _context.Languages.FirstOrDefault(a => a.Name == "English").LanguageId;
                _context.Freelancer_Language.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId && a.LanguageId == EnglishId).ProficiencyId = model.ProficiencyId.Value;
                var SecondLanguage = _context.Freelancer_Language.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId && a.LanguageId != EnglishId);
                SecondLanguage.ProficiencyId = model.Proficiency1Id.Value;
                _context.SaveChanges();
                return RedirectToAction("Profile");
            }
            return PartialView("EditLanguages", model);
        }

        public async Task<IActionResult> EditTitle()
        {
            var u = await _UserManager.GetUserAsync(User);
            var Freelancer = _context.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            EditTitleViewModel model = new EditTitleViewModel() { Title = Freelancer.Title };
            return PartialView("EditTitleModal",model);
        }

        [HttpPost]
        public async Task<IActionResult> EditTitle(EditTitleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await _UserManager.GetUserAsync(User);
                var Freelancer = _context.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                Freelancer.Title = model.Title;
                _context.SaveChanges();
                return RedirectToAction("Profile");
            }          
            return PartialView("EditTitleModal", model);
        }


        public async Task<IActionResult> EditOverview()
        {
            var u = await _UserManager.GetUserAsync(User);
            var Freelancer = _context.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            EditOverviewViewModel model = new EditOverviewViewModel() { Overview = Freelancer.Overview };
            return PartialView("EditOverviewModal", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditOverview(EditOverviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await _UserManager.GetUserAsync(User);
                var Freelancer = _context.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                Freelancer.Overview = model.Overview;
                _context.SaveChanges();
                return RedirectToAction("Profile");
            }
            return PartialView("EditOverviewModal", model);
        }

        public IActionResult AddEducation()
        {
            return PartialView("AddEducationModal");
        }

        [HttpPost]
        public async Task<IActionResult> AddEducation(AddEducationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await _UserManager.GetUserAsync(User);
                var Freelancer = _context.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                if (_context.Schools.FirstOrDefault(a => a.Name == model.School) == null)
                {
                    _context.Schools.Add(new School() { Name = model.School });
                }
                if (_context.AreasOfStudy.FirstOrDefault(a => a.Name == model.AreaOfStudy) == null)
                {
                    _context.AreasOfStudy.Add(new AreaOfStudy() { Name = model.AreaOfStudy });
                }
                if (_context.Degrees.FirstOrDefault(a => a.Name == model.Degree) == null)
                {
                    _context.Degrees.Add(new Degree() { Name = model.Degree });
                }
                _context.SaveChanges();
                var SchoolId = _context.Schools.FirstOrDefault(a => a.Name == model.School).SchoolId;
                var AreaId = _context.AreasOfStudy.FirstOrDefault(a => a.Name == model.AreaOfStudy).AreaId;
                var DegreeId = _context.Degrees.FirstOrDefault(a => a.Name == model.Degree).DegreeId;
                _context.Freelancer_Education.Add(new Freelancer_Education() { FreelancerId = Freelancer.FreelancerId, AreaId = AreaId, SchoolId = SchoolId, DegreeId = DegreeId, From = new DateTime(model.From.Value, 1, 1), To = new DateTime(model.To.Value, 1, 1), Description = model.Description });
                _context.SaveChanges();
                return RedirectToAction("Profile");
            }
            return PartialView("AddEducationModal",model);
        }

        public async Task<IActionResult> DeleteEducation(int AreaId, int SchoolId, int DegreeId, string FreelancerId)
        {            
            var Education = _context.Freelancer_Education.FirstOrDefault(a => a.FreelancerId == FreelancerId && a.AreaId == AreaId && a.DegreeId == DegreeId && a.SchoolId == SchoolId);
            _context.Freelancers.FirstOrDefault(a => a.FreelancerId == FreelancerId).Educations.Remove(Education);
            _context.Freelancer_Education.Remove(Education);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> EditEducation(int AreaId, int SchoolId, int DegreeId, string FreelancerId)
        {        
            AddEducationViewModel model = new AddEducationViewModel();
            var education = await _context.Freelancer_Education.Include(a => a.School).Include(a => a.AreaOfStudy).Include(a => a.Degree).FirstOrDefaultAsync(a => a.FreelancerId == FreelancerId && a.AreaId == AreaId && a.SchoolId == SchoolId && a.DegreeId == DegreeId);
            if (education != null)
            {
                model.School = education.School.Name;
                model.AreaOfStudy = education.AreaOfStudy.Name;
                model.Degree = education.Degree.Name;
                model.From = education.From.Year;
                model.To = education.To.Year;
                model.Description = education.Description;
                model.FreerlancerId = education.FreelancerId;
                model.AreaId = education.AreaId;
                model.DegreeId = education.DegreeId;
                model.SchoolId = education.SchoolId;
            }
            return PartialView("EditEducationModal", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditEducation(AddEducationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await _UserManager.GetUserAsync(User);
                var Freelancer = _context.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                if (_context.Schools.FirstOrDefault(a => a.Name == model.School) == null)
                {
                    _context.Schools.Add(new School() { Name = model.School });
                }
                if (_context.AreasOfStudy.FirstOrDefault(a => a.Name == model.AreaOfStudy) == null)
                {
                    _context.AreasOfStudy.Add(new AreaOfStudy() { Name = model.AreaOfStudy });
                }
                if (_context.Degrees.FirstOrDefault(a => a.Name == model.Degree) == null)
                {
                    _context.Degrees.Add(new Degree() { Name = model.Degree });
                }
                _context.SaveChanges();
                var SchoolId = _context.Schools.FirstOrDefault(a => a.Name == model.School).SchoolId;
                var AreaId = _context.AreasOfStudy.FirstOrDefault(a => a.Name == model.AreaOfStudy).AreaId;
                var DegreeId = _context.Degrees.FirstOrDefault(a => a.Name == model.Degree).DegreeId;
                var education = _context.Freelancer_Education.FirstOrDefault(a => a.FreelancerId == model.FreerlancerId && a.DegreeId == model.DegreeId && a.SchoolId == model.SchoolId && a.AreaId == model.AreaId);               
                _context.Freelancers.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId).Educations.Remove(education);
                _context.Freelancer_Education.Remove(education);
                await _context.SaveChangesAsync();
                _context.Freelancer_Education.Add(new Freelancer_Education() { FreelancerId = Freelancer.FreelancerId, AreaId = AreaId, SchoolId = SchoolId, DegreeId = DegreeId, From = new DateTime(model.From.Value, 1, 1), To = new DateTime(model.To.Value, 1, 1), Description = model.Description });
                await _context.SaveChangesAsync();
                return RedirectToAction("Profile");
            }
            return PartialView("EditEducationModal", model);
        }

        public IActionResult AddEmployement()
        {
            ViewBag.CountryId = new SelectList(_context.Countries, "CountryId", "Name");
            return PartialView("AddEmployementMdal");
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployement(AddEmployementViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await _UserManager.GetUserAsync(User);
                var Freelancer = _context.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                if (_context.Companies.FirstOrDefault(a => a.Name == model.Company) == null)
                {
                    _context.Companies.Add(new Company() { Name = model.Company });
                }
                if (_context.JobTitle.FirstOrDefault(a => a.Name == model.Title) == null)
                {
                    _context.JobTitle.Add(new JobTitle() { Name = model.Title });
                }            
                _context.SaveChanges();
                var CompanyId = _context.Companies.FirstOrDefault(a => a.Name == model.Company).CompanyId;
                var JobTitleId = _context.JobTitle.FirstOrDefault(a => a.Name == model.Title).JobTitleId;
                _context.Freelancer_Experience.Add(new Freelancer_Experience() { FreelancerId = Freelancer.FreelancerId, CompanyId = CompanyId, Location = model.Location, CountryId = model.CountryId.Value, JobTitleId = JobTitleId, From = new DateTime(model.FromYear, model.FromMonth, 1), To = new DateTime(model.ToYear, model.ToMonth, 1), Description = model.Description });
                _context.SaveChanges();
                return RedirectToAction("Profile");
            }
            return PartialView("AddEmployementMdal");
        }

        public async Task<IActionResult> DeleteEmployement(string FreelancerId, int CompanyId, int CountryId, int JobTitleId)
        {        
            var Employement = _context.Freelancer_Experience.FirstOrDefault(a => a.FreelancerId == FreelancerId && a.CompanyId == CompanyId && a.CountryId == CountryId && a.JobTitleId == JobTitleId);
            _context.Freelancers.FirstOrDefault(a => a.FreelancerId == FreelancerId).Experiences.Remove(Employement);
            _context.Companies.FirstOrDefault(a => a.CompanyId == CompanyId).FreelancerExperiences.Remove(Employement);
            _context.Countries.FirstOrDefault(a => a.CountryId == CountryId).FreelancerExperiences.Remove(Employement);
            _context.JobTitle.FirstOrDefault(a => a.JobTitleId == JobTitleId).FreelancerExperiences.Remove(Employement);
            _context.Freelancer_Experience.Remove(Employement);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> EditEmployement(string FreelancerId, int CompanyId, int CountryId, int JobTitleId)
        {
            var Employement = await _context.Freelancer_Experience.Include(a=>a.Company).Include(a=>a.JobTitle).FirstOrDefaultAsync(a => a.FreelancerId == FreelancerId && a.CompanyId == CompanyId && a.CountryId == CountryId && a.JobTitleId == JobTitleId);
            AddEmployementViewModel model = new AddEmployementViewModel();
            if (Employement != null)
            {
                model.Company = Employement.Company.Name;
                model.CountryId = Employement.CountryId;
                model.Description = Employement.Description;
                model.Location = Employement.Location;
                model.Title = Employement.JobTitle.Name;
                model.FromMonth = Employement.From.Month;
                model.FromYear = Employement.From.Year;
                model.ToMonth = Employement.To.Month;
                model.ToYear = Employement.To.Year;
            }
            ViewBag.CountryId = new SelectList(_context.Countries, "CountryId", "Name");
            return PartialView("EditEmployementModal", model);

        }



    }
}

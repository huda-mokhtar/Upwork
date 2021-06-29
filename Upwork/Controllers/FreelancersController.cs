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

namespace Upwork.Controllers
{
    [Authorize(Roles = "Freelancer")]
    public class FreelancersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly IWebHostEnvironment hosting;

        public FreelancersController(ApplicationDbContext context , UserManager<ApplicationUser> usermanager, IWebHostEnvironment _hosting)
        {
            _context = context;
            _UserManager = usermanager;
            hosting = _hosting;
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
            var Jobs = await _context.Jobs.Where(a => a.subCategoryId == freelancer.SubCategoryId && a.IsDraft == false).Include(a=>a.jobsSkills).Include(a => a.freelancer_Jobs).ToListAsync();
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
            ViewData["Skills"] = _context.JobsSkills.Select(s => s.skill).ToList();
            ViewData["SavesJobs"] = _context.Freelancer_Jobs.Include(a => a.Jobs).Where(a => a.FreelancerId == CurrentUser.Id && a.IsSaved ==true && a.Isdislike == false);
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
            var Job = _context.Jobs.Include(a => a.Client).Include(a => a.jobsSkills).Include(a => a.subCategory).FirstOrDefault(a => a.Id == Id && a.IsDraft == false && a.IsCanceled == false);
            if(Job == null)
            {
                return NotFound();
            }
            ViewData["Skills"] = _context.JobsSkills.Where(a => a.JobsId == Id).Select(a => a.skill);
            return View(Job);
        }


    }
}

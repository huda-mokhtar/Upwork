using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Upwork.Data;
using Upwork.Models;
using Upwork.Models.DbModels;

namespace Upwork.Controllers
{
    public class FreelancersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _UserManager;
        private ApplicationUser CurrentUser;
        public FreelancersController(ApplicationDbContext context , UserManager<ApplicationUser> usermanager)
        {
            _context = context;
            _UserManager = usermanager;
        }

        // GET: Freelancers
        public async Task<IActionResult> Index()
        {
            //CurrentUser = await _UserManager.GetUserAsync(User);
            var freelancer = await _context.Freelancers.Include(a=>a.SubCategory).Include(a=>a.Category).Include(a => a.Freelancer_Jobs).Include(a =>a.User).FirstOrDefaultAsync(a =>a.FreelancerId == "a123");
            if(freelancer == null)
            {
                return BadRequest();
            }
            var Jobs = await _context.Jobs.Where(a => a.subCategoryId == freelancer.SubCategoryId && a.IsDraft == false).Include(a=>a.jobsSkills).Include(a => a.freelancer_Jobs).ToListAsync();
            var Dislikejobs =  _context.Freelancer_Jobs.Where(a => a.Isdislike == true).Select(a => a.Jobs).ToList();
            var jobskills = _context.JobsSkills.Select(s => s.skill);
            ViewData["Skills"] = jobskills.ToList();
            ViewData["Freelancer"] = freelancer;
            return View(Jobs.Except(Dislikejobs));
        }

        // GET: Freelancers/Profile/5
        public async Task<IActionResult> Profile(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var freelancer = await _context.Freelancers.Include(a => a.Educations).Include(a => a.SubCategory).Include(a => a.Category).Include(a => a.Freelancer_Jobs).Include(a => a.User).Include(a => a.City).Include(a => a.Skills).Include(a => a.Languages).FirstOrDefaultAsync(a => a.FreelancerId == id);
            
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

        [HttpPost]
        public async Task<IActionResult> SearchForJob(string search)
        {
            var Freelancer = "a123";
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
            ViewData["SavesJobs"] = _context.Freelancer_Jobs.Include(a => a.Jobs).Where(a => a.FreelancerId == Freelancer && a.IsSaved ==true && a.Isdislike == false);
            return View(Job.Union(JobList));
        }

        public async Task<IActionResult> SaveJob(int id)
        {
            var FreelancerId = "a123";
            Freelancer_Job savedJobs = _context.Freelancer_Jobs.Where(a => a.JobsId == id && a.FreelancerId == FreelancerId).FirstOrDefault();
            if (savedJobs == null)
            {
                savedJobs = new Freelancer_Job() { FreelancerId = FreelancerId, JobsId = id ,IsSaved=true };
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
        
        public async Task<IActionResult> UnSaveJob(int id)
        {
            var FreelancerId = "a123";
            Freelancer_Job savedJobs = _context.Freelancer_Jobs.Where(a => a.JobsId == id && a.FreelancerId == FreelancerId &&a.IsSaved==true).FirstOrDefault();
            if (savedJobs != null)
            {
               savedJobs.IsSaved = false;
                _context.SaveChanges();
                
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DislikeJob(int id)
        {
            var FreelancerId = "a123";
            Freelancer_Job savedJobs = _context.Freelancer_Jobs.Where(a => a.JobsId == id && a.FreelancerId == FreelancerId ).FirstOrDefault();
            if (savedJobs == null)
            {
                savedJobs = new Freelancer_Job() { FreelancerId = FreelancerId, JobsId = id, Isdislike = true };
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
      
        public async Task<IActionResult> SubmitProposal(int ? Id)
        {
            if(Id == null)
            {
                return BadRequest();
            }

            var Freelancer = _context.Freelancers.Where(a => a.FreelancerId == "a123").FirstOrDefault();
            var Job = _context.Jobs.Where(a => a.Id == Id).FirstOrDefault();
            if(Freelancer == null || Job == null)
            {
                return NotFound();
            }
            Freelancer_Job job = new Freelancer_Job() { FreelancerId = Freelancer.FreelancerId, JobsId = Job.Id, IsProposal = true };
            _context.Freelancer_Jobs.Add(job);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Freelancers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancer = await _context.Freelancers.FindAsync(id);
            if (freelancer == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", freelancer.CategoryId);
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "Name", freelancer.CityId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "SubCategoryId", "Name", freelancer.SubCategoryId);
            ViewData["FreelancerId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", freelancer.FreelancerId);
            return View(freelancer);
        }

        // POST: Freelancers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("FreelancerId,CategoryId,SubCategoryId,ExperienceLevel,HourlyRate,Title,Overview,Image,CityId,Street,ZIP,PhoneNumber,VideoLink")] Freelancer freelancer)
        {
            if (id != freelancer.FreelancerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(freelancer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerExists(freelancer.FreelancerId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", freelancer.CategoryId);
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "Name", freelancer.CityId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "SubCategoryId", "Name", freelancer.SubCategoryId);
            ViewData["FreelancerId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", freelancer.FreelancerId);
            return View(freelancer);
        }

        // GET: Freelancers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var freelancer = await _context.Freelancers
                .Include(f => f.Category)
                .Include(f => f.City)
                .Include(f => f.SubCategory)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FreelancerId == id);
            if (freelancer == null)
            {
                return NotFound();
            }

            return View(freelancer);
        }

        // POST: Freelancers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var freelancer = await _context.Freelancers.FindAsync(id);
            _context.Freelancers.Remove(freelancer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FreelancerExists(string id)
        {
            return _context.Freelancers.Any(e => e.FreelancerId == id);
        }
    }
}

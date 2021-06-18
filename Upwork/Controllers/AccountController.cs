using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Data;
using Upwork.Models;
using Upwork.Models.ViewModels.Register;

namespace Upwork.Controllers
{
    public class AccountController : Controller
    {

        private ApplicationDbContext db;
        private SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IWebHostEnvironment hosting;

        public AccountController(ApplicationDbContext _db, 
            SignInManager<ApplicationUser> _signInManager, 
            UserManager<ApplicationUser> _userManager,
            RoleManager<IdentityRole> _roleManager,
            IWebHostEnvironment _hosting)
        {
            db = _db;
            signInManager = _signInManager;
            userManager = _userManager;
            roleManager = _roleManager;
            hosting = _hosting;
        }

       
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { Email = model.Email, UserName = model.Email };
                IdentityResult result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {                   
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Data");
                }
                foreach (var error in result.Errors)
                {
                    ViewBag.Message = "This email is already in use.";
                }
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Data()
        {
            var u = await userManager.GetUserAsync(User);
            ViewBag.Email = u.Email;
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");
            return View();         
        }

        [HttpPost]
        public async Task<IActionResult> Data(UserData model)
        {
            var u = await userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                if (model.Username != null)
                {
                    var oldUserWithSameUsername = db.Users.FirstOrDefault(a => a.UserName == model.Username);
                    if (oldUserWithSameUsername != null)
                    {
                        ViewBag.UsernameMessage = "This username is already in use";
                        ViewBag.Email = u.Email;
                        ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");
                        return View(model);
                    }
                    u.UserName = model.Username;
                    u.FirstName = model.FirstName;
                    u.LastName = model.LastName;
                    u.CountryId = model.CountryId;
                    u.SendMe = model.SendMe;
                    await userManager.AddPasswordAsync(u, model.Password);
                    db.Freelancers.Add(new Freelancer() { FreelancerId = u.Id });
                    db.SaveChanges();
                    return RedirectToAction("Category");
                }
                else
                {
                    u.FirstName = model.FirstName;
                    u.LastName = model.LastName;
                    u.CountryId = model.CountryId;
                    u.SendMe = model.SendMe;
                    await userManager.AddPasswordAsync(u, model.Password);
                    await userManager.AddToRoleAsync(u, "Client");
                    // redirect to client home page
                    return Content("Client home here...");
                }               
            }
            ViewBag.Email = u.Email;
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");
            return View(model);
        }

        public List<SubCategory> GetSubCategories(int id)
        {
            return db.SubCategories.Where(a => a.CategoryId == id).ToList();
        }


        [Authorize]
        public async Task<IActionResult> Category()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image != null)
            {
                ViewBag.Image = Freelancer.Image;
            }
            return View();           
        }

        [HttpPost]
        public IActionResult Category(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                string Id = HttpContext.Session.GetString("UserId");
                var freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == Id);
                freelancer.CategoryId = model.CategoryId;
                freelancer.SubCategoryId = model.SubCategoryId;
                db.SaveChanges();
                return RedirectToAction("Expertise");
            }
        //    ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
         //   return View(model);
            return RedirectToAction("Expertise");

        }

        [Authorize]
        public async Task<IActionResult> Expertise()
        {
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image != null)
            {
                ViewBag.Image = Freelancer.Image;
            }
            return View(db.Skills.Take(15).ToList());    
        }

        [HttpPost]
        public IActionResult Expertise(int x)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("ExpertiseLevel");
            }
            return RedirectToAction("ExpertiseLevel");

//            return View();
        }

        [Authorize]
        public async Task<IActionResult> ExpertiseLevelAsync()
        {
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image != null)
            {
                ViewBag.Image = Freelancer.Image;
            }
            return View();
        }

        [HttpPost]
        public IActionResult ExpertiseLevel(int x)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Education");
            }
            return RedirectToAction("Education");
        //    return View();
        }

        [Authorize]
        public async Task<IActionResult> EducationAsync()
        {
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image != null)
            {
                ViewBag.Image = Freelancer.Image;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Education(int x)
        {
            return RedirectToAction("Employment");
        }

       

        [Authorize]
        public async Task<IActionResult> Employment()
        {
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image != null)
            {
                ViewBag.Image = Freelancer.Image;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Employment(int x)
        {
            return RedirectToAction("Languages");
        }

        [Authorize]
        public async Task<IActionResult> Languages()
        {
            ViewBag.ProficiencyId = new SelectList(db.Language_Proficiency, "ProficiencyId", "Name");
            ViewBag.LanguageId = new SelectList(db.Languages, "LanguageId", "Name");
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image != null)
            {
                ViewBag.Image = Freelancer.Image;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Languages(int x)
        {
            return RedirectToAction("HourlyRate");
        }

        [Authorize]
        public async Task<IActionResult> HourlyRate()
        {
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image != null)
            {
                ViewBag.Image = Freelancer.Image;
            }
            return View();
        }

        [HttpPost]
        public IActionResult HourlyRate(HourlyRateViewModel model)
        {
            return RedirectToAction("Overview");    
        }

        [Authorize]
        public async Task<IActionResult> Overview()
        {
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image != null)
            {
                ViewBag.Image = Freelancer.Image;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Overview(int x)
        {
            return RedirectToAction("ProfilePhoto");
        }

        [Authorize]
        public async Task<IActionResult> ProfilePhoto()
        {
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image != null)
            {
                ViewBag.Image = Freelancer.Image;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProfilePhoto(ProfilePhotoViewModel model)
        {
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (ModelState.IsValid)
            {
                string FileName = string.Empty;
                if (model.File != null)
                {
                    string Uploads = Path.Combine(hosting.WebRootPath, "ProfilePhotos");
                    FileName = Freelancer.FreelancerId + "." + model.File.FileName.Split('.')[model.File.FileName.Split('.').Length-1];
                    string FullPath = Path.Combine(Uploads, FileName);
                    if (Freelancer.Image != null)
                    {
                        System.IO.File.Delete(FullPath);
                    }
                    model.File.CopyTo(new FileStream(FullPath, FileMode.Create));
                }
                Freelancer.Image = FileName;
                db.SaveChanges();
                return RedirectToAction("Location");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Location()
        {
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image != null)
            {
                ViewBag.Image = Freelancer.Image;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Location(int x)
        {
            return RedirectToAction("Phone");
        }

        [Authorize]
        public async Task<IActionResult> Phone()
        {
            ViewBag.Countries = db.Countries.ToList();
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image != null)
            {
                ViewBag.Image = Freelancer.Image;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Phone(int x)
        {
            return View();
        }


    }
}

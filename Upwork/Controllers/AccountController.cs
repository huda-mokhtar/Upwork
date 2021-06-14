using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
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
        public AccountController(ApplicationDbContext _db, SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> _userManager)
        {
            db = _db;
            signInManager = _signInManager;
            userManager = _userManager;
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
                    HttpContext.Session.SetString("UserId", user.Id);
                    return RedirectToAction("Data");
                }
            }
            return View(model);
        }

        public IActionResult Data()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                string Id = HttpContext.Session.GetString("UserId");
                var user = db.Users.FirstOrDefault(a => a.Id == Id);
                ViewBag.Email = user.Email;
                ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");               
                return View();
            }
           //   return View();
            return RedirectToAction("Signup");
            //  return LocalRedirect("~/Identity/Account/Register");
        }

        [HttpPost]
        public async Task<IActionResult> Data(UserData model)
        {
            if (ModelState.IsValid)
            {
                string Id = HttpContext.Session.GetString("UserId");
                var user = db.Users.FirstOrDefault(a => a.Id == Id);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.CountryId = model.CountryId;
                user.SendMe = model.SendMe;
                await userManager.ChangePasswordAsync(user, user.PasswordHash, model.Password);
                if (model.Username != null)
                {
                    user.UserName = model.Username;
                    db.Freelancers.Add(new Models.Freelancer() { FreelancerId = Id });
                 //   await userManager.AddToRoleAsync(user, "Freelancer");
                }
                else
                {

                    //   await userManager.AddToRoleAsync(user, "Client");
                }
                db.SaveChanges();
                return RedirectToAction("Category");
            }
            return View(model);
        }

        public List<SubCategory> GetSubCategories(int id)
        {
            return db.SubCategories.Where(a => a.CategoryId == id).ToList();
        }

        public IActionResult Category()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
                return View();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");

            return View();
           //   return RedirectToAction("Signup");
            //    return LocalRedirect("~/Identity/Account/Register");
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


        public IActionResult Expertise()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return View(db.Skills.Take(15).ToList());
            }
            return View(db.Skills.Take(15).ToList());
         //   return RedirectToAction("Signup");
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

        public IActionResult ExpertiseLevel()
        {
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

        public IActionResult Education()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Education(int x)
        {
            return RedirectToAction("Employment");
        }

       

        public IActionResult Employment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Employment(int x)
        {
            return RedirectToAction("Languages");
        }

        public IActionResult Languages()
        {
            ViewBag.ProficiencyId = new SelectList(db.Language_Proficiency, "ProficiencyId", "Name");
            ViewBag.LanguageId = new SelectList(db.Languages, "LanguageId", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Languages(int x)
        {
            return RedirectToAction("HourlyRate");
        }

        public IActionResult HourlyRate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult HourlyRate(HourlyRateViewModel model)
        {
            return RedirectToAction("Overview");    
        }

        public IActionResult Overview()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Overview(int x)
        {
            return RedirectToAction("ProfilePhoto");
        }

        public IActionResult ProfilePhoto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProfilePhoto(int id)
        {
            return RedirectToAction("Location");
        }

        public IActionResult Location()
        {
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Location(int x)
        {
            return RedirectToAction("Phone");
        }


        public IActionResult Phone()
        {
            ViewBag.Countries = db.Countries.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Phone(int x)
        {
            return View();
        }


    }
}

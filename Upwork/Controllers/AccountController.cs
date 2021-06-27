using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Upwork.Data;
using Upwork.Models;
using Upwork.Models.ViewModels.Register;
using Microsoft.AspNetCore.Authentication;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Security.Claims;
using System.Net;

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
                    // Generate Email confirmation token             
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                    new { userId = user.Id, token = token }, Request.Scheme);
                    //
                    await signInManager.SignOutAsync();
                    return Content(confirmationLink);
                    /*
                    // Send Emails using SendGrid
                    var apikey = "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";
                    var client = new SendGridClient(apikey);
                    var from = new EmailAddress("itiupwork@gmail.com","Upwork");
                    var to = new EmailAddress(user.Email);
                    var subject = "Confirm your email";
                    var plainTextContent = "";
                    var htmlContent = "<strong> Click on the link to confirm your email: " +confirmationLink + " </strong>";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    var response = await client.SendEmailAsync(msg);
                    await signInManager.SignOutAsync();
                    return RedirectToAction("VerifyEmail", new { id = user.Id });                                    
               */
                    }
                foreach (var error in result.Errors)
                {
                    ViewBag.Message = "This email is already in use.";
                }
            }
            return View(model);
        }

        //Verify email 
        public async Task<IActionResult> VerifyEmail(string id)
        {
            
            if (id == null)
            {
                return RedirectToAction("SignUp");
            }

            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction("SignUp");
            }

            if (user.EmailConfirmed)
            {
                return RedirectToAction("Data");
            }
            ViewBag.Email = user.Email;
            return View();

        }

        // Confirm email
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            
            if (userId == null || token == null)
            {
                return RedirectToAction("SignUp");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("SignUp");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: true);
                return View();
            }

            return RedirectToAction("SignUp");

        }

        // SignUp with Google
        public IActionResult GoogleSignup()
        {
            var redircetUrl = Url.Action("GoogleSignupCallback", "Account");
            var Properties = signInManager.ConfigureExternalAuthenticationProperties("Google",redircetUrl);
            return new ChallengeResult("Google",Properties);
        }

        public async Task<IActionResult> GoogleSignupCallback(string remoteError = null)
        {
            if (remoteError != null)
            {
                ViewBag.GoogleMessage = "Error from Google provider!";
                return View("SignUp");
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ViewBag.GoogleMessage = "Error loading external login information!";
                return View("SignUp");
            }

            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true,bypassTwoFactor: true);
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(email);

            if (signInResult.Succeeded)
            {          
                if (await userManager.IsInRoleAsync(user, "Freelancer"))
                {
                    return RedirectToAction("Index", "Freelancers");
                }
                else if (await userManager.IsInRoleAsync(user, "Client"))
                {
                    return RedirectToAction("Index", "Client");
                }
                else if (await userManager.IsInRoleAsync(user, "Admin"))
                {
                    return Content("Admin home page...");
                }
                else if (db.Freelancers.FirstOrDefault(a => a.FreelancerId == user.Id) != null)
                {
                    return RedirectToAction("Phone");
                }
                else
                {
                    return RedirectToAction("DataExternal");
                }      
            }
            else
            {         
                if (email != null)
                {              
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                            LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                            EmailConfirmed = true
                        };

                        await userManager.CreateAsync(user);
                        
                    }
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("DataExternal");
                }

                ViewBag.GoogleMessage = "Error loading external login information!";
                return View("SignUp");
            }   
        }

        // Login Google
        public IActionResult GoogleLogin()
        {
            var redircetUrl = Url.Action("GoogleLoginCallback", "Account");
            var Properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redircetUrl);
            return new ChallengeResult("Google", Properties);
        }

        public async Task<IActionResult> GoogleLoginCallback(string remoteError = null)
        {
            if (remoteError != null)
            {
                ViewBag.GoogleMessage = "Error from Google provider!";
                return View("Login");
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ViewBag.GoogleMessage = "Error loading external login information!";
                return View("Login");
            }

            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {            
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = await userManager.FindByEmailAsync(email);
                if (await userManager.IsInRoleAsync(user, "Freelancer"))
                {
                    return RedirectToAction("Index", "Freelancers");
                }
                else if (await userManager.IsInRoleAsync(user, "Client"))
                {
                    return RedirectToAction("Index", "Client");
                }
                else if (await userManager.IsInRoleAsync(user, "Admin"))
                {
                    return Content("Admin home page...");
                }
                else if (db.Freelancers.FirstOrDefault(a => a.FreelancerId == user.Id) != null)
                {
                    return RedirectToAction("Phone");
                }
                else
                {
                    return RedirectToAction("DataExternal");
                }             
            }
            else
            {      
                ViewBag.GoogleMessage = "Invalid login attempt!";
                return View("Login");
            }         
        }

        // Extrnal User data 
        [Authorize]
        public async Task<IActionResult> DataExternal()
        {         
            var u = await userManager.GetUserAsync(User);
            if (db.UserLogins.FirstOrDefault(a => a.UserId == u.Id) == null)
            {
                return RedirectToAction("Data");
            }
            if (db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id) != null)
            {
                return RedirectToAction("SignUp");
            }
            ViewBag.Email = u.Email;
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DataExternal(ExternalUserData model)
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
                    u.CountryId = model.CountryId;
                    u.SendMe = model.SendMe;
                    db.Freelancers.Add(new Freelancer() { FreelancerId = u.Id });
                    db.SaveChanges();
                    return RedirectToAction("GettingStarted");
                }
                else
                {            
                    u.CountryId = model.CountryId;
                    u.SendMe = model.SendMe;
                    await userManager.AddToRoleAsync(u, "Client");
                    return RedirectToAction("Index", "Client");
                    // redirect to client home page
                    //  return Content("Client home here...");
                }
            }
            ViewBag.Email = u.Email;
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");
            return View(model);
        }


        // User data
        [Authorize]
        public async Task<IActionResult> Data()
        {
            var u = await userManager.GetUserAsync(User);
            if (db.UserLogins.FirstOrDefault(a=>a.UserId == u.Id) != null)
            {
                return RedirectToAction("DataExternal");
            }
            if (db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id) != null)
            {
                return RedirectToAction("SignUp");
            }
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
                    return RedirectToAction("GettingStarted");
                }
                else
                {
                    u.FirstName = model.FirstName;
                    u.LastName = model.LastName;
                    u.CountryId = model.CountryId;
                    u.SendMe = model.SendMe;
                    await userManager.AddPasswordAsync(u, model.Password);                  
                    await userManager.AddToRoleAsync(u, "Client");
                    return RedirectToAction("Index", "Client");
                    // redirect to client home page
                  //  return Content("Client home here...");
                }               
            }
            ViewBag.Email = u.Email;
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");
            return View(model);    
        }


        //Check state of freelancer (done or not)
        private async Task CheckState()
        {
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.CategoryId != null)
            {
                ViewBag.Category = "Done";
            }
            if (db.Freelancer_Skill.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId) != null)
            {
                ViewBag.Expertise = "Done";
            }
            if (Freelancer.ExperienceLevel != null)
            {
                ViewBag.ExpertiseLevel = "Done";
            }
            if (db.Freelancer_Education.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId) != null)
            {
                ViewBag.Education = "Done";
            }
            if (db.Freelancer_Experience.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId) != null)
            {
                ViewBag.Employement = "Done";
            }
            if (db.Freelancer_Language.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId) != null)
            {
                ViewBag.Languages = "Done";
            }
            if (Freelancer.HourlyRate != null)
            {
                ViewBag.HourlyRate = "Done";
            }
            if (Freelancer.Overview != null)
            {
                ViewBag.Overview = "Done";
            }
            if (Freelancer.Image != null)
            {
                ViewBag.ProfilePhoto = Freelancer.Image;
            }
            if (Freelancer.CityId != null)
            {
                ViewBag.Location = "Done";
            }
            if (Freelancer.PhoneNumber != null)
            {
                ViewBag.Phone = "Done";
            }
        }

        //Authorize Freelancer
        private async Task<bool> NotAuthorized()
        {
            var u = await userManager.GetUserAsync(User);
            if (db.Freelancers.FirstOrDefault(a=>a.FreelancerId == u.Id) == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        //Getting Started

        [Authorize]
        public async Task<IActionResult> GettingStarted()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            await CheckState();
            return View();
        }

        //Category

        public List<SubCategory> GetSubCategories(int id)
        {           
            return db.SubCategories.Where(a => a.CategoryId == id).ToList();
        }


        [Authorize]
        public async Task<IActionResult> Category()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            await CheckState();
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            CategoryViewModel model = new CategoryViewModel() { CategoryId = Freelancer.CategoryId, SubCategoryId = Freelancer.SubCategoryId };
            if (Freelancer.CategoryId != null)
            {
                ViewBag.SubCategoryId = new SelectList(db.SubCategories.Where(a=>a.CategoryId == Freelancer.CategoryId), "SubCategoryId", "Name");
            }
            return View(model);           
        }

        [HttpPost]
        public async Task<IActionResult> Category(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await userManager.GetUserAsync(User);
                var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                Freelancer.CategoryId = model.CategoryId;
                Freelancer.SubCategoryId = model.SubCategoryId;
                db.SaveChanges();
                return RedirectToAction("Expertise");
            }
            await CheckState();
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View(model);

        }


        //Expertise
        public List<Skill> GetSkills()
        {
            return db.Skills.ToList();
        }

        [Authorize]
        public async Task<IActionResult> Expertise()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.CategoryId == null)
            {
                return RedirectToAction("Category");
            }
            await CheckState(); 
            Dictionary<Skill, bool> model = new Dictionary<Skill, bool>();
            List<Skill> Top15Skills = db.Skills.Take(15).ToList();
            foreach (var item in Top15Skills)
            {
                if (db.Freelancer_Skill.FirstOrDefault(a=>a.SkillId == item.SkillId && a.FreelancerId == Freelancer.FreelancerId) != null)
                {
                    model.Add(item, true);
                }
                else
                {
                    model.Add(item, false);
                }
            }
            return View(model);    
        }

        [HttpPost]
        public async Task<IActionResult> Expertise(Dictionary<int,bool> Suggestedskill)
        {           
            if (ModelState.IsValid)
            {
                var u = await userManager.GetUserAsync(User);
                var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                var FreelancerSkills = db.Freelancer_Skill.Where(a => a.FreelancerId == Freelancer.FreelancerId).ToList();
                foreach (var item in FreelancerSkills)
                {
                    db.Freelancer_Skill.Remove(item);
                }
                db.SaveChanges();
                foreach (var item in Suggestedskill)
                {
                    if (item.Value==true)
                    {
                        await db.Freelancer_Skill.AddAsync(new Freelancer_Skill() { FreelancerId = Freelancer.FreelancerId, SkillId = item.Key });                                       
                    }
                }
                db.SaveChanges();               
                return RedirectToAction("ExpertiseLevel");
            }
            return View();
        }

        //Expertise level

        [Authorize]
        public async Task<IActionResult> ExpertiseLevel()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (db.Freelancer_Skill.FirstOrDefault(a=>a.FreelancerId == Freelancer.FreelancerId) == null)
            {
                return RedirectToAction("Expertise");
            }
            await CheckState();           
            ViewBag.FreelancerLevel = Freelancer.ExperienceLevel;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ExpertiseLevel(ExpertiseLevelViewModel model)
        {   
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (model.ExperienceLevel == "One")
            {
                Freelancer.ExperienceLevel = "Entry";
            }
            else if (model.ExperienceLevel == "Two")
            {
                Freelancer.ExperienceLevel = "Intermediate";
            }
            else if (model.ExperienceLevel == "Three")
            {
                Freelancer.ExperienceLevel = "Expert";
            }
            db.SaveChanges();
            return RedirectToAction("Education");          
        }

        //Education

        [Authorize]
        public async Task<IActionResult> Education()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.ExperienceLevel == null)
            {
                return RedirectToAction("ExpertiseLevel");
            }
            await CheckState();           
            return View(db.Freelancer_Education.Include(a=>a.AreaOfStudy).Include(a=>a.School).Include(a=>a.Degree).Where(a=>a.FreelancerId == Freelancer.FreelancerId).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Education(int x)
        {
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (db.Freelancer_Education.FirstOrDefault(a=>a.FreelancerId == Freelancer.FreelancerId)== null)
            {
                ViewBag.Message = "Add at least one item";
                await CheckState();          
                return View(db.Freelancer_Education.Include(a => a.AreaOfStudy).Include(a => a.School).Include(a => a.Degree).Where(a => a.FreelancerId == Freelancer.FreelancerId).ToList());
            }
            else
            {
                return RedirectToAction("Employment");
            }
        }

       [Authorize]
        public async Task<IActionResult> AddEducation()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            return PartialView("AddEducationModal");
        }

        
        [HttpPost]
        public async Task<IActionResult> AddEducation(AddEducationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await userManager.GetUserAsync(User);
                var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                db.Schools.Add(new School() { Name=model.School});
                db.AreasOfStudy.Add(new AreaOfStudy() { Name = model.AreaOfStudy });
                db.Degrees.Add(new Degree() { Name= model.Degree});
                db.SaveChanges();
                var SchoolId = db.Schools.FirstOrDefault(a => a.Name == model.School).SchoolId;
                var AreaId = db.AreasOfStudy.FirstOrDefault(a => a.Name == model.AreaOfStudy).AreaId;
                var DegreeId = db.Degrees.FirstOrDefault(a => a.Name == model.Degree).DegreeId;
                db.Freelancer_Education.Add(new Freelancer_Education() { FreelancerId=Freelancer.FreelancerId , AreaId = AreaId , SchoolId = SchoolId, DegreeId = DegreeId,From = new DateTime(model.From.Value, 1, 1) , To = new DateTime(model.To.Value, 1, 1), Description = model.Description}) ;
                db.SaveChanges();
                return RedirectToAction("Education", "Account");
            }
            return PartialView("AddEducationModal");
        }

              
        [Authorize]
        public async Task<IActionResult> EditEducation(int AreaId, int SchoolId, int DegreeId, string FreelancerId)
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            AddEducationViewModel model = new AddEducationViewModel();
            var education = db.Freelancer_Education.Include(a=>a.School).Include(a=>a.AreaOfStudy).Include(a=>a.Degree).FirstOrDefault(a => a.FreelancerId == FreelancerId && a.AreaId == AreaId && a.SchoolId == SchoolId && a.DegreeId == DegreeId);
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
            return PartialView("EditEducationModal",model);
        }

        [HttpPost]
        public async Task<IActionResult> EditEducation(AddEducationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await userManager.GetUserAsync(User);
                var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                if (db.Schools.FirstOrDefault(a=>a.Name == model.School) == null)
                {
                    db.Schools.Add(new School() { Name = model.School });
                }
                if (db.AreasOfStudy.FirstOrDefault(a=>a.Name == model.AreaOfStudy) == null)
                {
                    db.AreasOfStudy.Add(new AreaOfStudy() { Name = model.AreaOfStudy });
                }
                if (db.Degrees.FirstOrDefault(a=>a.Name == model.Degree) == null)
                {
                    db.Degrees.Add(new Degree() { Name = model.Degree });
                }
                db.SaveChanges();
                var SchoolId = db.Schools.FirstOrDefault(a => a.Name == model.School).SchoolId;
                var AreaId = db.AreasOfStudy.FirstOrDefault(a => a.Name == model.AreaOfStudy).AreaId;
                var DegreeId = db.Degrees.FirstOrDefault(a => a.Name == model.Degree).DegreeId;
                var education = db.Freelancer_Education.FirstOrDefault(a => a.FreelancerId == model.FreerlancerId && a.DegreeId == model.DegreeId && a.SchoolId == model.SchoolId && a.AreaId == model.AreaId);
                education.AreaId = AreaId;
                education.DegreeId = DegreeId;
                education.SchoolId = SchoolId;
                education.From = new DateTime(model.From.Value, 1, 1);
                education.To = new DateTime(model.To.Value, 1, 1);
                education.Description = model.Description;
                db.SaveChanges();
                return RedirectToAction("Education", "Account");
            }
            return PartialView("AddEducationModal");
        }


        [Authorize]
        public async Task<IActionResult> DeleteEducation(int AreaId, int SchoolId, int DegreeId, string FreelancerId)
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            var Education = db.Freelancer_Education.FirstOrDefault(a => a.FreelancerId == FreelancerId && a.AreaId == AreaId && a.DegreeId == DegreeId && a.SchoolId == SchoolId);
            db.Freelancers.FirstOrDefault(a => a.FreelancerId == FreelancerId).Educations.Remove(Education);
            db.Freelancer_Education.Remove(Education);
            db.SaveChanges();
            return RedirectToAction("Education");
        }

        //Employement
        [Authorize]
        public async Task<IActionResult> Employment()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (db.Freelancer_Education.FirstOrDefault(a=>a.FreelancerId == Freelancer.FreelancerId) == null)
            {
                return RedirectToAction("Education");
            }
            await CheckState();        
            return View(db.Freelancer_Experience.Include(a=>a.JobTitle).Include(a=>a.Company).Where(a=>a.FreelancerId== Freelancer.FreelancerId).ToList());
        }
       

        [HttpPost]
        public async Task<IActionResult> Employment(int x)
        {
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (db.Freelancer_Experience.FirstOrDefault(a=>a.FreelancerId == Freelancer.FreelancerId) == null)
            {
                await CheckState();
                ViewBag.Message = "Add at least one item.";
                return View(db.Freelancer_Experience.Include(a => a.JobTitle).Include(a => a.Company).Where(a => a.FreelancerId == Freelancer.FreelancerId).ToList());
            }
            return RedirectToAction("Languages");
        }

        [Authorize]
        public async Task<IActionResult> AddEmployement()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");
            return PartialView("AddEmployementModal");
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployement(AddEmployementViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await userManager.GetUserAsync(User);
                var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                db.Companies.Add(new Company() { Name = model.Company });
                db.JobTitle.Add(new JobTitle() { Name = model.Title });    
                db.SaveChanges();
                var CompanyId = db.Companies.FirstOrDefault(a => a.Name == model.Company).CompanyId;
                var JobTitleId = db.JobTitle.FirstOrDefault(a => a.Name == model.Title).JobTitleId;
                db.Freelancer_Experience.Add(new Freelancer_Experience() { FreelancerId = Freelancer.FreelancerId, CompanyId = CompanyId, Location = model.Location, CountryId = model.CountryId.Value, JobTitleId = JobTitleId, From = new DateTime(model.FromYear, model.FromMonth, 1), To = new DateTime(model.ToYear, model.ToMonth, 1), Description = model.Description });
                db.SaveChanges();
                return RedirectToAction("Employment", "Account");
            }
            return PartialView("AddEmployementModal");
        }

        [Authorize]
        public async Task<IActionResult> DeleteEmployement(string FreelancerId,int CompanyId,int CountryId,int JobTitleId)
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            var Employement = db.Freelancer_Experience.FirstOrDefault(a => a.FreelancerId == FreelancerId && a.CompanyId == CompanyId && a.CountryId == CountryId && a.JobTitleId == JobTitleId);
            db.Freelancers.FirstOrDefault(a => a.FreelancerId == FreelancerId).Experiences.Remove(Employement);
            db.Companies.FirstOrDefault(a => a.CompanyId == CompanyId).FreelancerExperiences.Remove(Employement);
            db.Countries.FirstOrDefault(a => a.CountryId == CountryId).FreelancerExperiences.Remove(Employement);
            db.JobTitle.FirstOrDefault(a => a.JobTitleId == JobTitleId).FreelancerExperiences.Remove(Employement);
            db.Freelancer_Experience.Remove(Employement);
            db.SaveChanges();
            return RedirectToAction("Employment");
        }

        //Languages
        [Authorize]
        public async Task<IActionResult> Languages()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (db.Freelancer_Experience.FirstOrDefault(a=>a.FreelancerId == Freelancer.FreelancerId) == null)
            {
                return RedirectToAction("Employment");
            }
            await CheckState();  
            var EnglishId = db.Languages.FirstOrDefault(a => a.Name == "English").LanguageId;
            LanguagesViewModel model = new LanguagesViewModel();
            if (db.Freelancer_Language.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId && a.LanguageId == EnglishId) != null)
            {
               model.ProficiencyId = db.Freelancer_Language.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId && a.LanguageId == EnglishId).ProficiencyId;                             
            }
            if (db.Freelancer_Language.FirstOrDefault(a=>a.FreelancerId == Freelancer.FreelancerId && a.LanguageId != EnglishId) != null)
            {
                var SecondLanguage = db.Freelancer_Language.FirstOrDefault(a => a.FreelancerId == Freelancer.FreelancerId && a.LanguageId != EnglishId);
                model.Language1Id = SecondLanguage.LanguageId;
                model.Proficiency1Id = SecondLanguage.ProficiencyId;
            }
            ViewBag.ProficiencyId = new SelectList(db.Language_Proficiency, "ProficiencyId", "Name");
            ViewBag.LanguageId = new SelectList(db.Languages.Where(a => a.LanguageId != EnglishId), "LanguageId", "Name");
            return View(model);     
        }

        [HttpPost]
        public async Task<IActionResult> Languages(LanguagesViewModel model)
        {
            var EnglishId = db.Languages.FirstOrDefault(a => a.Name == "English").LanguageId;
            if (ModelState.IsValid)
            {
                if (model.Language1Id != null && model.Proficiency1Id == null)
                {
                    await CheckState();
                    ViewBag.ProficiencyId = new SelectList(db.Language_Proficiency, "ProficiencyId", "Name");
                    ViewBag.LanguageId = new SelectList(db.Languages.Where(a => a.LanguageId != EnglishId), "LanguageId", "Name");
                    ViewBag.Message = "You must select proficiency level.";
                    return View(model);
                }
                if (model.Language1Id == null && model.Proficiency1Id != null)
                {
                    await CheckState();
                    ViewBag.ProficiencyId = new SelectList(db.Language_Proficiency, "ProficiencyId", "Name");
                    ViewBag.LanguageId = new SelectList(db.Languages.Where(a => a.LanguageId != EnglishId), "LanguageId", "Name");
                    ViewBag.LanguageMessage = "You must select Language.";
                    return View(model);
                }
                var u = await userManager.GetUserAsync(User);
                var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                var FreelancerLanguages = db.Freelancer_Language.Where(a => a.FreelancerId == Freelancer.FreelancerId).ToList();
                foreach (var item in FreelancerLanguages)
                {
                    db.Freelancer_Language.Remove(item);
                }
                db.SaveChanges();
                db.Freelancer_Language.Add(new Freelancer_Language() { FreelancerId = Freelancer.FreelancerId, LanguageId = EnglishId, ProficiencyId = model.ProficiencyId.Value });
                if (model.Language1Id != null && model.Proficiency1Id != null)
                {
                    db.Freelancer_Language.Add(new Freelancer_Language() { FreelancerId = Freelancer.FreelancerId, LanguageId = model.Language1Id.Value, ProficiencyId = model.Proficiency1Id.Value });
                }
                db.SaveChanges();
                return RedirectToAction("HourlyRate");
            }
            await CheckState();
            ViewBag.ProficiencyId = new SelectList(db.Language_Proficiency, "ProficiencyId", "Name");
            ViewBag.LanguageId = new SelectList(db.Languages.Where(a => a.LanguageId != EnglishId), "LanguageId", "Name");
            return View(model);
        }

        //Hourly Rate

        [Authorize]
        public async Task<IActionResult> HourlyRate()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (db.Freelancer_Language.FirstOrDefault(a=>a.FreelancerId == Freelancer.FreelancerId) == null)
            {
                return RedirectToAction("Languages");
            }
            await CheckState(); 
            HourlyRateViewModel model = new HourlyRateViewModel() { HourlyRate = Freelancer.HourlyRate };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> HourlyRate(HourlyRateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await userManager.GetUserAsync(User);
                var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                Freelancer.HourlyRate = model.HourlyRate;
                db.SaveChanges();
                return RedirectToAction("Overview");
            }
            return View(model);
        }

        //Overview

        [Authorize]
        public async Task<IActionResult> Overview()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.HourlyRate == null)
            {
                return RedirectToAction("HourlyRate");
            }
            await CheckState();         
            OverviewViewModel model = new OverviewViewModel() { Title = Freelancer.Title, Overview = Freelancer.Overview };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Overview(OverviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await userManager.GetUserAsync(User);
                var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                Freelancer.Title = model.Title;
                Freelancer.Overview = model.Overview;
                db.SaveChanges();
                return RedirectToAction("ProfilePhoto");
            }
            return View(model);
        }

        //Profile Photo

        [Authorize]
        public async Task<IActionResult> ProfilePhoto()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Overview == null)
            {
                return RedirectToAction("Overview");
            }
            await CheckState();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProfilePhoto(ProfilePhotoViewModel model)
        {
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image == null && model.File == null)
            {
                await CheckState();
                ViewBag.ErrorMessage = "Profile photo is required!";
                return View(model);
            } 
            if (ModelState.IsValid)
            {
                string FileName = Freelancer.Image;
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

        //Location

        [Authorize]
        public async Task<IActionResult> Location()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.Include(a=>a.City).FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Image == null)
            {
                return RedirectToAction("ProfilePhoto");
            }
            await CheckState();
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");
            LocationViewModel model = new LocationViewModel();
            if (Freelancer.CityId != null)
            {
                model.City = Freelancer.City.Name;
                model.CountryId = Freelancer.City.CountryId;
            }
            if (Freelancer.Street != null)
            {
                model.Street = Freelancer.Street;
            }
            if (Freelancer.ZIP != null)
            {
                model.ZIP = Freelancer.ZIP;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Location(LocationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await userManager.GetUserAsync(User);
                var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                db.Cities.Add(new City() { Name = model.City, CountryId = model.CountryId.Value });
                db.SaveChanges();
                var CityId = db.Cities.FirstOrDefault(a => a.Name == model.City).CityId;
                Freelancer.CityId = CityId;
                Freelancer.Street = model.Street;
                Freelancer.ZIP = model.ZIP;
                db.SaveChanges();
                return RedirectToAction("Phone");
            }
            await CheckState();
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "Name");
            return View(model);
        }

        //Phone

        [Authorize]
        public async Task<IActionResult> Phone()
        {
            if (await NotAuthorized())
            {
                return RedirectToAction("Data");
            }
            var u = await userManager.GetUserAsync(User);
            var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
            if (Freelancer.Street == null)
            {
                return RedirectToAction("Location");
            }
            await CheckState();
            ViewBag.Countries = db.Countries.ToList();
            PhoneViewModel model = new PhoneViewModel();
            if (Freelancer.PhoneCountryId != null)
            {
                model.CountryId = Freelancer.PhoneCountryId.Value;
            }
            if (Freelancer.PhoneNumber != null)
            {
                model.Phone = int.Parse(Freelancer.PhoneNumber);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Phone(PhoneViewModel model)
        {
            if (ModelState.IsValid)
            {
                var u = await userManager.GetUserAsync(User);
                var Freelancer = db.Freelancers.FirstOrDefault(a => a.FreelancerId == u.Id);
                Freelancer.PhoneCountryId = model.CountryId;
                Freelancer.PhoneNumber = model.Phone.ToString();
                await userManager.AddToRoleAsync(u, "Freelancer");
                db.SaveChanges();
                return RedirectToAction("Index", "Freelancers");
               // return Content("Freelancer home page...");
            }
            await CheckState();
            ViewBag.Countries = db.Countries.ToList();
            return View(model);
        }

        //Login
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {        
                var user = await userManager.FindByEmailAsync(model.Email);              
                if (user != null)
                {
                    var result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);
                    if (result.Succeeded)
                    {
                        if (await userManager.IsInRoleAsync(user, "Freelancer"))
                        {
                            return RedirectToAction("Index", "Freelancers");
                        }
                        else if (await userManager.IsInRoleAsync(user, "Client"))
                        {
                            return RedirectToAction("Index", "Client");
                        }
                        else if (await userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return Content("Admin home page...");
                        }
                        else if (db.Freelancers.FirstOrDefault(a=>a.FreelancerId == user.Id) !=null)
                        {
                            return RedirectToAction("Phone");
                        }
                        else
                        {
                            return RedirectToAction("Data");                            
                        }
                    }           
                }
                ViewBag.Message = "Invalid login attempt";
                return View(model);                                                          
            }
            return View(model);
        }


        //Logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
           // return Content("Home page...");
        }


        [Authorize(Roles ="Client")]
        public IActionResult Client()
        {
            return Content("Client home page...");
        }

        [Authorize(Roles = "Freelancer")]
        public IActionResult Freelancer()
        {
            return Content("Freelancer home page...");
        }


    }
}

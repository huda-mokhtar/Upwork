using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Data;
using Upwork.Models;
using Upwork.Models.ViewModels.Admin;

namespace Upwork.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db;
        private SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private IWebHostEnvironment hosting;

        public AdminController(ApplicationDbContext _db,
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

        private async Task SetLayoutData()
        {
            var u = await userManager.GetUserAsync(User);
            ViewBag.AdminName = u.FirstName + " " + u.LastName;
        }

        //Admin home
        public async Task<IActionResult> Index()
        {
            await SetLayoutData();          
            ViewBag.Freelancers = db.Freelancers.Count();
            ViewBag.Clients = db.Clients.Count();
            ViewBag.Projects = db.Projects.Count();
            ViewBag.Jobs = db.Jobs.Count();              
            return View(db.Freelancers.Include(a=>a.User).Include(a=>a.PhoneCountry).Include(a=>a.Freelancer_Jobs).ToList());
        }

        //Freelancers
        public async Task<IActionResult> Freelancers()
        {
            await SetLayoutData();
            return View(db.Freelancers.Include(a=>a.User).Include(a=>a.PhoneCountry).ToList());
        }

        [Route("Admin/Freelancers/Details")]
        public async Task<IActionResult> FreelancerDetails(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = db.Freelancers.Include(a=>a.User).Include(a=>a.PhoneCountry).Include(a=>a.Category).Include(a=>a.SubCategory).Include(a=>a.Projects).Include(a=>a.Freelancer_Jobs).FirstOrDefault(a => a.FreelancerId == id);
            if (model == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            ViewBag.Skills = db.Freelancer_Skill.Include(a => a.Skill).Where(a => a.FreelancerId == id).ToList();
            return View(model);
        }

        //Clients
        public async Task<IActionResult> Clients()
        {
            await SetLayoutData();
            return View(db.Clients.Include(a => a.User).Include(a=>a.User.Country ).Include(a=>a.client_Projects).Include(a=>a.Jobs).ToList());
        }

        [Route("Admin/Clients/Details")]
        public async Task<IActionResult> ClientDetails(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = db.Clients.Include(a=>a.User).Include(a=>a.User.Country).Include(a=>a.client_Projects).Include(a=>a.Jobs).FirstOrDefault(a => a.ClientId == id);
            if (model == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            return View(model);
        }

        //Jobs
        public async Task<IActionResult> Jobs()
        {
            await SetLayoutData();
            return View(db.Jobs.Include(a=>a.subCategory).Include(a=>a.Client.User).Include(a=>a.Language_Proficiency).ToList());
        }

        [Route("Admin/Jobs/Details")]
        public async Task<IActionResult> JobDetails(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = db.Jobs.Include(a=>a.subCategory).Include(a=>a.Client.User).Include(a=>a.Language_Proficiency).FirstOrDefault(a => a.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            ViewBag.Skills = db.JobsSkills.Include(a => a.skill).Where(a => a.JobsId == id).ToList();
            return View(model);
        }

        //Projects

        public async Task<IActionResult> Projects()
        {
            await SetLayoutData();
            return View(db.Projects.Include(a=>a.client_Projects).Include(a=>a.Freelancer.User).Include(a=>a.SubCategory).Include(a=>a.Skills).ToList());
        }


        [Route("Admin/Projects/Details")]
        public async Task<IActionResult> ProjectDetails(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = db.Projects.Include(a => a.client_Projects).Include(a => a.Freelancer.User).Include(a => a.SubCategory).FirstOrDefault(a=>a.ProjectId == id);
            if (model == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            ViewBag.Skills = db.ProjectSkills.Include(a => a.Skill).Where(a => a.ProjectId == id).ToList();
            return View(model);
        }

        //Categories
        public async Task<IActionResult> Categories()
        {
            await SetLayoutData();
            return View(db.Categories.ToList());
        }

        [Route("Admin/Categories/Create")]
        public async Task<IActionResult> CreateCategory()
        {
            await SetLayoutData();
            return View();
        }

        [Route("Admin/Categories/Create")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(new Category() { Name = model.Name });
                await db.SaveChangesAsync();
                return RedirectToAction("Categories");
            }
            await SetLayoutData();
            return View(model);
        }


        [Route("Admin/Categories/Edit")]
        public async Task<IActionResult> EditCategory(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var Category = db.Categories.FirstOrDefault(a => a.CategoryId == id);
            if (Category == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            return View(Category);
        }

        [Route("Admin/Categories/Edit")]
        [HttpPost]
        public async Task<IActionResult> EditCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                var category = db.Categories.FirstOrDefault(a => a.CategoryId == model.CategoryId);
                category.Name = model.Name;
                await db.SaveChangesAsync();
                return RedirectToAction("Categories");
            }
            await SetLayoutData();
            return View(model);
        }

        [Route("Admin/Categories/Details")]
        public async Task<IActionResult> CategoryDetails(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var Category = db.Categories.Include(a=>a.SubCategories).FirstOrDefault(a => a.CategoryId == id);
            if (Category == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            return View(Category);
        }


        //SubCategories
        public async Task<IActionResult> SubCategories()
        {
            await SetLayoutData();
            return View(db.SubCategories.Include(a=>a.Category).ToList());
        }

        [Route("Admin/SubCategories/Create")]
        public async Task<IActionResult> CreateSubCategory()
        {
            await SetLayoutData();
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View();
        }

        [Route("Admin/SubCategories/Create")]
        [HttpPost]
        public async Task<IActionResult> CreateSubCategory(SubCategory model)
        {
            if (ModelState.IsValid)
            {
                db.SubCategories.Add(new SubCategory() { Name = model.Name, CategoryId = model.CategoryId });
                await db.SaveChangesAsync();
                return RedirectToAction("SubCategories");
            }
            await SetLayoutData();
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View(model);
        }

        [Route("Admin/SubCategories/Edit")]
        public async Task<IActionResult> EditSubCategory(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var SubCategory = db.SubCategories.FirstOrDefault(a=>a.SubCategoryId == id);
            if (SubCategory == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View(SubCategory);
        }

        [Route("Admin/SubCategories/Edit")]
        [HttpPost]
        public async Task<IActionResult> EditSubCategory(SubCategory model)
        {
            if (ModelState.IsValid)
            {
                var SubCategory = db.SubCategories.FirstOrDefault(a => a.SubCategoryId == model.SubCategoryId);
                SubCategory.Name = model.Name;
                SubCategory.CategoryId = model.CategoryId;
                await db.SaveChangesAsync();
                return RedirectToAction("SubCategories");
            }
            await SetLayoutData();
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View(model);
        }

        [Route("Admin/SubCategories/Details")]
        public async Task<IActionResult> SubCategoryDetails(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var SubCategory = db.SubCategories.Include(a => a.Category).FirstOrDefault(a => a.SubCategoryId == id);
            if (SubCategory == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            return View(SubCategory);
        }


        //Countries
        public async Task<IActionResult> Countries()
        {
            await SetLayoutData();
            return View(db.Countries.ToList());
        }

        [Route("Admin/Countries/Create")]
        public async Task<IActionResult> CreateCountry()
        {
            await SetLayoutData();
            return View();
        }

        [Route("Admin/Countries/Create")]
        [HttpPost]
        public async Task<IActionResult> CreateCountry(Country model)
        {
            if (ModelState.IsValid)
            {
                db.Countries.Add(new Country() { Name = model.Name, Key = model.Key });
                await db.SaveChangesAsync();
                return RedirectToAction("Countries");
            }
            await SetLayoutData();
            return View(model);
        }

        [Route("Admin/Countries/Edit")]
        public async Task<IActionResult> EditCountry(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = db.Countries.FirstOrDefault(a => a.CountryId == id);
            if (model == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            return View(model);
        }

        [Route("Admin/Countries/Edit")]
        [HttpPost]
        public async Task<IActionResult> EditCountry(Country model)
        {
            if (ModelState.IsValid)
            {
                var Country = db.Countries.FirstOrDefault(a => a.CountryId == model.CountryId);
                Country.Name = model.Name;
                Country.Key = model.Key;
                await db.SaveChangesAsync();
                return RedirectToAction("Countries");
            }
            await SetLayoutData();
            return View(model);
        }

        [Route("Admin/Countries/Details")]
        public async Task<IActionResult> CountryDetails(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = db.Countries.FirstOrDefault(a => a.CountryId == id);
            if (model == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            return View(model);
        }       
                    
        
        //Languages
        public async Task<IActionResult> Languages()
        {
            await SetLayoutData();
            return View(db.Languages.ToList());
        }

        [Route("Admin/Languages/Create")]
        public async Task<IActionResult> CreateLanguage()
        {
            await SetLayoutData();
            return View();
        }

        [Route("Admin/Languages/Create")]
        [HttpPost]
        public async Task<IActionResult> CreateLanguage(Language model)
        {
            if (ModelState.IsValid)
            {
                db.Languages.Add(new Language() { Name = model.Name });
                await db.SaveChangesAsync();
                return RedirectToAction("Languages");
            }
            await SetLayoutData();
            return View(model);
        }

        [Route("Admin/Languages/Edit")]
        public async Task<IActionResult> EditLanguage(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = db.Languages.FirstOrDefault(a => a.LanguageId == id);
            if (model == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            return View(model);
        }

        [Route("Admin/Languages/Edit")]
        [HttpPost]
        public async Task<IActionResult> EditLanguage(Language model)
        {
            if (ModelState.IsValid)
            {
                var Language = db.Languages.FirstOrDefault(a => a.LanguageId == model.LanguageId);
                Language.Name = model.Name;
                await db.SaveChangesAsync();
                return RedirectToAction("Languages");
            }
            await SetLayoutData();
            return View(model);
        }

        [Route("Admin/Languages/Details")]
        public async Task<IActionResult> LanguageDetails(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = db.Languages.FirstOrDefault(a => a.LanguageId == id);
            if (model == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            return View(model);
        }
        //Proficiencies
        public async Task<IActionResult> Proficiencies()
        {
            await SetLayoutData();
            return View(db.Language_Proficiency.ToList());
        }

        [Route("Admin/Proficiencies/Create")]
        public async Task<IActionResult> CreateProficiency()
        {
            await SetLayoutData();
            return View();
        }

        [Route("Admin/Proficiencies/Create")]
        [HttpPost]
        public async Task<IActionResult> CreateProficiency(Language_Proficiency model)
        {
            if (ModelState.IsValid)
            {
                db.Language_Proficiency.Add(new Language_Proficiency() { Name = model.Name, Description = model.Description });
                await db.SaveChangesAsync();
                return RedirectToAction("Proficiencies");
            }
            await SetLayoutData();
            return View(model);
        }

        [Route("Admin/Proficiencies/Edit")]
        public async Task<IActionResult> EditProficiency(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = db.Language_Proficiency.FirstOrDefault(a => a.ProficiencyId == id);
            if (model == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            return View(model);
        }

        [Route("Admin/Proficiencies/Edit")]
        [HttpPost]
        public async Task<IActionResult> EditProficiency(Language_Proficiency model)
        {
            if (ModelState.IsValid)
            {
                var Proficiency = db.Language_Proficiency.FirstOrDefault(a => a.ProficiencyId == model.ProficiencyId);
                Proficiency.Name = model.Name;
                Proficiency.Description = model.Description;
                await db.SaveChangesAsync();
                return RedirectToAction("Proficiencies");
            }
            await SetLayoutData();
            return View(model);
        }

        [Route("Admin/Proficiencies/Details")]
        public async Task<IActionResult> ProficiencyDetails(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = db.Language_Proficiency.FirstOrDefault(a => a.ProficiencyId == id);
            if (model == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            return View(model);
        }
        //Skills 
        public async Task<IActionResult> Skills()
        {
            await SetLayoutData();
            return View(db.Skills.Include(a=>a.SubCategory).ToList());
        }

        [Route("Admin/Skills/Create")]
        public async Task<IActionResult> CreateSkill()
        {
            await SetLayoutData();
            ViewBag.SubCategoryId = new SelectList(db.SubCategories, "SubCategoryId", "Name");
            return View();
        }

        [Route("Admin/Skills/Create")]
        [HttpPost]
        public async Task<IActionResult> CreateSkill(SkillViewModel model)
        {
            if (ModelState.IsValid)
            {
                db.Skills.Add(new Skill() { Name = model.Name, SubCategoryId = model.SubCategoryId });
                await db.SaveChangesAsync();
                return RedirectToAction("Skills");
            }
            await SetLayoutData();
            ViewBag.SubCategoryId = new SelectList(db.SubCategories, "SubCategoryId", "Name");
            return View(model);
        }

        [Route("Admin/Skills/Edit")]
        public async Task<IActionResult> EditSkill(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var Skill = db.Skills.FirstOrDefault(a => a.SkillId == id);
            if (Skill == null)
            {
                return NotFound();
            }
            var model = new SkillViewModel() { SkillId = Skill.SkillId, Name = Skill.Name, SubCategoryId = Skill.SubCategoryId };
            await SetLayoutData();
            ViewBag.SubCategoryId = new SelectList(db.SubCategories, "SubCategoryId", "Name");
            return View(model);
        }


        [Route("Admin/Skills/Edit")]
        [HttpPost]
        public async Task<IActionResult> EditSkill(SkillViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Skill = db.Skills.FirstOrDefault(a => a.SkillId == model.SkillId);
                Skill.Name = model.Name;
                Skill.SubCategoryId = model.SubCategoryId;
                await db.SaveChangesAsync();
                return RedirectToAction("Skills");
            }
            await SetLayoutData();
            ViewBag.SubCategoryId = new SelectList(db.SubCategories, "SubCategoryId", "Name");
            return View(model);
        }

        [Route("Admin/Skills/Details")]
        public async Task<IActionResult> SkillDetails(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var model = db.Skills.Include(a=>a.SubCategory).FirstOrDefault(a => a.SkillId == id);
            if (model == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            return View(model);
        }

        // Roles
        public async Task<IActionResult> Roles()
        {
            await SetLayoutData();
            return View(db.Roles.ToList());
        }

        [Route("Admin/Roles/Create")]
        public async Task<IActionResult> CreateRole()
        {
            await SetLayoutData();
            return View();
        }

        [Route("Admin/Roles/Create")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {                 
                await roleManager.CreateAsync(new IdentityRole() { Name = model.Name});
                await db.SaveChangesAsync();
                return RedirectToAction("Roles");
            }
            await SetLayoutData();
            return View(model);
        }


        [Route("Admin/Roles/Edit")]
        public async Task<IActionResult> EditRole(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var Role = db.Roles.FirstOrDefault(a => a.Id == id);
            if (Role == null)
            {
                return NotFound();
            }
            var model = new RoleViewModel() { Id = Role.Id, Name = Role.Name };
            await SetLayoutData();
            return View(model);
        }

        [Route("Admin/Roles/Edit")]
        [HttpPost]
        public async Task<IActionResult> EditRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Role = db.Roles.FirstOrDefault(a => a.Id == model.Id);
                await roleManager.SetRoleNameAsync(Role, model.Name);
                await db.SaveChangesAsync();
                return RedirectToAction("Roles");
            }
            await SetLayoutData();
            return View(model);
        }

        [Route("Admin/Roles/Details")]
        public async Task<IActionResult> RoleDetails(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var Role = db.Roles.FirstOrDefault(a => a.Id == id);
            if (Role == null)
            {
                return NotFound();
            }
            await SetLayoutData();
            ViewBag.NumberOfUsers = await db.UserRoles.CountAsync(a => a.RoleId == Role.Id);
            return View(Role);
        }
    }
}

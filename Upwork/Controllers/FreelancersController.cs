using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Upwork.Data;
using Upwork.Models;

namespace Upwork.Controllers
{
    public class FreelancersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FreelancersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Profile()
        {
            return View();
        }

        // GET: Freelancers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Freelancers.Include(f => f.Category).Include(f => f.City).Include(f => f.SubCategory).Include(f => f.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Freelancers/Details/5
        public async Task<IActionResult> Details(string id)
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

        // GET: Freelancers/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "Name");
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "SubCategoryId", "Name");
            ViewData["FreelancerId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            return View();
        }

        // POST: Freelancers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FreelancerId,CategoryId,SubCategoryId,ExperienceLevel,HourlyRate,Title,Overview,Image,CityId,Street,ZIP,PhoneNumber,VideoLink")] Freelancer freelancer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(freelancer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", freelancer.CategoryId);
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "Name", freelancer.CityId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "SubCategoryId", "Name", freelancer.SubCategoryId);
            ViewData["FreelancerId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", freelancer.FreelancerId);
            return View(freelancer);
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

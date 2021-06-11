using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Upwork.Data;
using Upwork.Models;

namespace Upwork.Controllers
{
    public class ProjectQuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectQuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //Get Question
        public async Task<IActionResult> GetQuestion()
        {
            return PartialView();
        }

        [HttpPost]
        //Post Question
        public async Task<IActionResult> GetQuestion(ProjectQuestion model)
        {
            if (HttpContext.Session.GetString("ProjectId") != null)
            {
                var projectId = int.Parse(HttpContext.Session.GetString("ProjectId"));
                model.ProjectId = projectId;
                if (ModelState.IsValid)
                {
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    return PartialView();
                }
            }
            return View(model);
        }
        // GET: ProjectQuestions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProjectQuestions.Include(p => p.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProjectQuestions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectQuestion = await _context.ProjectQuestions
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectQuestion == null)
            {
                return NotFound();
            }

            return View(projectQuestion);
        }

        // GET: ProjectQuestions/Create
        public IActionResult Create()
        {
                       
           return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( ProjectQuestion projectQuestion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectQuestion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            /*ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "Title", projectQuestion.ProjectId);*/
            return View(projectQuestion);
        }
        
            // GET: ProjectQuestions/Edit/5
            public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectQuestion = await _context.ProjectQuestions.FindAsync(id);
            if (projectQuestion == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "Title", projectQuestion.ProjectId);
            return View(projectQuestion);
        }

        // POST: ProjectQuestions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QuestionContent,Answer,ProjectId")] ProjectQuestion projectQuestion)
        {
            if (id != projectQuestion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectQuestion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectQuestionExists(projectQuestion.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "Title", projectQuestion.ProjectId);
            return View(projectQuestion);
        }

        // GET: ProjectQuestions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectQuestion = await _context.ProjectQuestions
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectQuestion == null)
            {
                return NotFound();
            }

            return View(projectQuestion);
        }

        // POST: ProjectQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectQuestion = await _context.ProjectQuestions.FindAsync(id);
            _context.ProjectQuestions.Remove(projectQuestion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectQuestionExists(int id)
        {
            return _context.ProjectQuestions.Any(e => e.Id == id);
        }
    }
}

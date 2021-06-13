using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Data;

namespace Upwork.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostenviroment;

        public ClientController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostenviroment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PostJob()
        {
            return View();
        }
        public IActionResult PostJobTitle()
        {
            return View();
        }
        public IActionResult PostJobSkills()
        {
            return View();
        }
        public IActionResult PostJobScope()
        {
            return View();
        }
        public IActionResult PostJobBudget()
        {
            return View();
        }
        public IActionResult ReviewJobPosting()
        {
            return View();
        }
    }
}

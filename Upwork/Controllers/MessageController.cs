using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Data;
using Upwork.Models;
using Upwork.Models.MessageModels;
using Upwork.services.MessageServices;

namespace Upwork.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        public readonly IChat _IChat;
        private readonly UserManager<ApplicationUser> _userManager;
        public MessageController( UserManager<ApplicationUser> userManager, IChat chat, ApplicationDbContext context)
        {
            _userManager = userManager;
            _IChat = chat;
            _context = context;
        }

        public async Task<IActionResult> Index(string Id)
        {
            var CurrentUser = await _userManager.GetUserAsync(User);
            if (Id != null)
            {
                var Reciver = _context.Users.FirstOrDefault(a => a.Id == Id);
                List<string> UsersResiverId = new List<string>();
                List<ApplicationUser> Users = new List<ApplicationUser>();
                var ListPeopel = _context.Messages.Where(a => a.UserId == CurrentUser.Id);
                foreach (var item in ListPeopel)
                {
                    if (!UsersResiverId.Contains(item.ReceiverId))
                    {
                        UsersResiverId.Add(item.ReceiverId);
                    }
                }
                foreach (var i in UsersResiverId)
                {
                    Users.Add(_context.Users.FirstOrDefault(a => a.Id == i));
                }
                ViewBag.ListPeopel = Users;
                ViewBag.CurrentUserName = CurrentUser.FirstName;
                ViewBag.Reciver = Reciver;
                var Messages = _IChat.GetMessageses(CurrentUser.Id, Id);
                return View(Messages);
            }
            var AllMessages = _context.Messages.Where(a => a.UserId == CurrentUser.Id || a.ReceiverId == CurrentUser.Id);
            if (AllMessages != null)
            {
                var fristchatId = AllMessages.FirstOrDefault(a => a.UserId == CurrentUser.Id).ReceiverId;
                if (fristchatId ==null)
                {
                    fristchatId= AllMessages.FirstOrDefault(a => a.ReceiverId == CurrentUser.Id).UserId;
                }
                return RedirectToAction("Index", "Message",new {Id= fristchatId });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Message message)
        {
            message.UserName = User.Identity.Name;
            var Sender = await _userManager.GetUserAsync(User);
            message.UserId = Sender.Id;
            message.When = DateTime.Now;
            await _IChat.AddMessage(message);
            return Ok();
        }
    }
}

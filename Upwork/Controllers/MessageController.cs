using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Models;
using Upwork.Models.MessageModels;
using Upwork.services.MessageServices;

namespace Upwork.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        public readonly IChat _IChat;
        private readonly UserManager<ApplicationUser> _userManager;
        public MessageController( UserManager<ApplicationUser> userManager, IChat chat)
        {
            _userManager = userManager;
            _IChat = chat;
        }

        public async Task<IActionResult> Index(string Id)
        {
            var CurrentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = CurrentUser.UserName;
                ViewBag.RecevierId = Id;
            }
            var Messages = _IChat.GetMessageses(CurrentUser.Id, Id);
            return View(Messages);

        }

        [HttpPost]
        public async Task<IActionResult> Create(Message message)
        {
            //if (ModelState.IsValid)
            //{

            message.UserName = User.Identity.Name;
            var Sender = await _userManager.GetUserAsync(User);
            message.UserId = Sender.Id;
            await _IChat.AddMessage(message);
            return Ok();

            //}
            //return Error();
        }
    }
}

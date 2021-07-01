using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Models.MessageModels;

namespace Upwork.Hubs
{
    public class ChatHub:Hub
    {
        public async Task SendMessage(Message message, string user) =>
        await Clients.Users(user).SendAsync("receiveMessage", message);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Models.MessageModels;

namespace Upwork.services.MessageServices
{
    public interface IChat
    {
        Task AddMessage(Message message);
        IQueryable<Message> GetMessageses(string UserId, string ReceiverId);
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Upwork.Data;
using Upwork.Models.MessageModels;

namespace Upwork.services.MessageServices
{
    public class Chat : IChat
    {
        private readonly ApplicationDbContext _context;

        public Chat(ApplicationDbContext context) => _context = context;

        public async Task AddMessage(Message message)
        {
            await _context.Messages.AddAsync(message);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
        }

        public IQueryable<Message> GetMessageses(string UserId, string ReceiverId)
        {

            return _context.Messages.Where(a => (a.UserId == UserId && a.ReceiverId == ReceiverId) || (a.UserId == ReceiverId && a.ReceiverId == UserId));
        }
    }
}


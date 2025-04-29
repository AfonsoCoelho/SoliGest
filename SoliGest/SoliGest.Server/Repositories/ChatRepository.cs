using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;      
using SoliGest.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoliGest.Server.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly SoliGestServerContext _ctx;
        public ChatRepository(SoliGestServerContext ctx) => _ctx = ctx;

        public async Task SaveAsync(ChatMessage msg)
        {
            _ctx.ChatMessages.Add(msg);
            await _ctx.SaveChangesAsync();
        }

        public Task<List<ChatMessage>> GetConversationAsync(string userA, string userB)
        {
            return _ctx.ChatMessages
                .Where(m =>
                    (m.FromUserId == userA && m.ToUserId == userB) ||
                    (m.FromUserId == userB && m.ToUserId == userA))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}

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
        private readonly SoliGestServerContext _db;

        public ChatRepository(SoliGestServerContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Conversation>> GetConversationsFor(string userId)
        {
            return await _db.Conversations
                .Include(c => c.Contact)      
                .Include(c => c.Messages)     
                .Where(c => c.Users.Any(u => u.Id == userId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Contact>> GetAvailableContacts(string userId)
        {
            return await _db.Users
                .Where(u => u.Id != userId)
                .Select(u => new Contact { Id = u.Id, Name = u.Name })
                .ToListAsync();
        }

        public async Task SaveMessage(string senderId, string receiverId, string content, DateTime timestamp)
        {
            var conv = await _db.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c =>
                    c.Users.Any(u => u.Id == senderId) &&
                    c.Users.Any(u => u.Id == receiverId));

            if (conv == null)
            {
                conv = new Conversation
                {
                    CreatedAt = DateTime.UtcNow,
                    Users = new List<User>
                    {
                        new User { Id = senderId },
                        new User { Id = receiverId }
                    },
                    Messages = new List<Message>()
                };
                _db.Conversations.Add(conv);
            }

            conv.Messages.Add(item: new Message
            {
                SenderId = senderId,
                Sender = _db.Users.Find(senderId),
                ReceiverId = receiverId,
                Receiver = _db.Users.Find(receiverId),
                Content = content,
                Timestamp = timestamp,
                ConversationId = conv.Id,
                Conversation = conv
            });

            await _db.SaveChangesAsync();
        }
    }
}

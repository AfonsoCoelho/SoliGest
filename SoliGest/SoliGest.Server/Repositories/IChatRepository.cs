using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoliGest.Server.Models;

namespace SoliGest.Server.Repositories
{
    public interface IChatRepository
    {
        Task<IEnumerable<Conversation>> GetConversationsFor(string userId);
        Task<IEnumerable<Contact>> GetAvailableContacts(string userId);
        Task SaveMessage(string senderId, string receiverId, string content, DateTime timestamp);
    }
}
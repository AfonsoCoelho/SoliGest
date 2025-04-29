using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoliGest.Server.Models;

namespace SoliGest.Server.Repositories
{
    public interface IChatRepository
    {
        Task SaveAsync(ChatMessage msg);
        Task<List<ChatMessage>> GetConversationAsync(string userA, string userB);
    }
}
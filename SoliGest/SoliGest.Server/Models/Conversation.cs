using System;
using System.Collections.Generic;

namespace SoliGest.Server.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public string ContactId { get; set; }
        public Contact Contact { get; set; }          
        public ICollection<Message> Messages { get; set; }
        public ICollection<User> Users { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
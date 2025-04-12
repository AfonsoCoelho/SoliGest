namespace SoliGest.Server.Models
{
    public class Conversation
    {
        public int Id { get; set; } 
        public required string UserId { get; set; }
        public required User User { get; set; }
        public required string ContactId { get; set; }
        public required User Contact { get; set; } 

        public ICollection<Message> Messages { get; set; } // Mensagens da conversa
    }
}

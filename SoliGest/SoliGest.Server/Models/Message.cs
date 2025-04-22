namespace SoliGest.Server.Models
{
    public class Message
    {
        public int Id { get; set; } 
        public required string Content { get; set; } 
        public DateTime Timestamp { get; set; }
        public required string SenderId { get; set; }
        public required User Sender { get; set; }
        public required string ReceiverId { get; set; }
        public required User Receiver { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
    }
}

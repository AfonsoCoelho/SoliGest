namespace SoliGest.Server.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string FromUserId { get; set; } = "";
        public string ToUserId { get; set; } = "";
        public string Text { get; set; } = "";
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}

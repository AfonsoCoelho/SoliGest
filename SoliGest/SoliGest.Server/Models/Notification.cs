namespace SoliGest.Server.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Type { get; set; }
        public required string Message { get; set; }
    }
}

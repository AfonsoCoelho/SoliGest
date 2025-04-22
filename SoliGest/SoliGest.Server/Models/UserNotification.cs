namespace SoliGest.Server.Models
{
    public class UserNotification
    {
        public int UserNotificationId { get; set; }
        public virtual required User User { get; set; }
        public required string UserId { get; set; }
        public virtual required Notification Notification { get; set; }
        public int NotificationId { get; set; }
        public DateTime ReceivedDate { get; set; }
        public bool IsRead { get; set; }
        public string? Link { get; set; }
    }
}

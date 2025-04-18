using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Models;

namespace SoliGest.Server.Data
{
    public class UserNotificationSeeder
    {
        public static async Task SeedUserNotificationsAsync(SoliGestServerContext context)
        {
            if (!context.UserNotification.Any())
            {
                User user = await context.Users.FirstOrDefaultAsync(u => u.Email.Equals("technician@mail.com"));
                Notification notification = await context.Notification.FirstOrDefaultAsync(n => n.Id == 1);
                UserNotification userNotification1 = new UserNotification
                {
                    UserNotificationId = 0,
                    User = user,
                    UserId = user.Id,
                    Notification = notification,
                    NotificationId = notification.Id,
                    ReceivedDate = DateTime.Now,
                    isRead = false
                };

                await context.AddAsync(userNotification1);

                UserNotification userNotification2 = new UserNotification
                {
                    UserNotificationId = 0,
                    User = user,
                    UserId = user.Id,
                    Notification = notification,
                    NotificationId = notification.Id,
                    ReceivedDate = DateTime.Now,
                    isRead = false
                };

                await context.AddAsync(userNotification2);

                UserNotification userNotification3 = new UserNotification
                {
                    UserNotificationId = 0,
                    User = user,
                    UserId = user.Id,
                    Notification = notification,
                    NotificationId = notification.Id,
                    ReceivedDate = DateTime.Now,
                    isRead = false
                };

                await context.AddAsync(userNotification3);

                await context.SaveChangesAsync();
            }
        }
    }
}

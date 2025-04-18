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
                Notification notification1 = await context.Notification.FindAsync(1);
                UserNotification userNotification1 = new UserNotification
                {
                    UserNotificationId = 0,
                    User = user,
                    UserId = user.Id,
                    Notification = notification1,
                    NotificationId = notification1.Id,
                    ReceivedDate = DateTime.Now,
                    IsRead = false
                };

                await context.AddAsync(userNotification1);


                Notification notification2 = await context.Notification.FindAsync(2);

                UserNotification userNotification2 = new UserNotification
                {
                    UserNotificationId = 0,
                    User = user,
                    UserId = user.Id,
                    Notification = notification2,
                    NotificationId = notification2.Id,
                    ReceivedDate = DateTime.Now,
                    IsRead = false
                };

                await context.AddAsync(userNotification2);

                Notification notification3 = await context.Notification.FindAsync(3);

                UserNotification userNotification3 = new UserNotification
                {
                    UserNotificationId = 0,
                    User = user,
                    UserId = user.Id,
                    Notification = notification3,
                    NotificationId = notification3.Id,
                    ReceivedDate = DateTime.Now,
                    IsRead = false
                };

                await context.AddAsync(userNotification3);

                await context.SaveChangesAsync();
            }
        }
    }
}

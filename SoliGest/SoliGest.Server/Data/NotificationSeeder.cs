using Microsoft.AspNetCore.Identity;
using SoliGest.Server.Models;
using SoliGest.Server.Data;
using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Data
{
    public class NotificationSeeder
    {
        public static async Task SeedNotificationsAsync(SoliGestServerContext context)
        {
            if (!context.Notification.Any())
            {
                Notification notification1 = new Notification
                {
                    Type = "Info",
                    Message = "Tem uma nova assistência técnica atribuída."
                };

                await context.AddAsync(notification1);

                Notification notification2 = new Notification
                {
                    Type = "Info",
                    Message = "Assistência técnica 99 solucionada!"
                };

                await context.AddAsync(notification2);

                Notification notification3 = new Notification
                {
                    Type = "Info",
                    Message = "Uma assistência técnica foi atualizada!"
                };

                await context.AddAsync(notification3);

                await context.SaveChangesAsync();
            }
        }
    }
}

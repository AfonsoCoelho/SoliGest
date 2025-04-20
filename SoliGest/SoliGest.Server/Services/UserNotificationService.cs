using Microsoft.AspNetCore.Mvc;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using static SoliGest.Server.Controllers.UserNotificationsController;

namespace SoliGest.Server.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly SoliGestServerContext _context;

        public UserNotificationService(SoliGestServerContext context)
        {
            _context = context;
        }

        public async Task PostUserNotification(UserNotificationUpdateModel model)
        {
            try
            {
                User user = await _context.FindAsync<User>(model.UserId);
                if (user == null)
                    throw new Exception("O utilizador com o Id fornecido não foi encontrado.");

                Notification notification = await _context.FindAsync<Notification>(model.NotificationId);
                if (notification == null)
                    throw new Exception("A notificação com o Id fornecido não foi encontrada.");

                UserNotification userNotification = new UserNotification
                {
                    UserNotificationId = 0,
                    User = user,
                    UserId = model.UserId,
                    Notification = notification,
                    NotificationId = model.NotificationId,
                    ReceivedDate = DateTime.Now,
                    IsRead = false
                };
                _context.UserNotification.Add(userNotification);
                await _context.SaveChangesAsync();

            }
            catch
            {
                throw new Exception("Erro.");
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using static SoliGest.Server.Controllers.UserNotificationsController;

namespace SoliGest.Server.Services
{
    public interface IUserNotificationService
    {
        Task PostUserNotification(UserNotificationUpdateModel model);
    }
}

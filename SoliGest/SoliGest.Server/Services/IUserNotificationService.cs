using Microsoft.AspNetCore.Mvc;
using static SoliGest.Server.Controllers.UserNotificationsController;

namespace SoliGest.Server.Services
{
    /// <summary>
    /// Interface para o serviço de notificações de usuário.
    /// Define as operações relacionadas ao gerenciamento de notificações para usuários.
    /// </summary>
    public interface IUserNotificationService
    {
        /// <summary>
        /// Cria ou atualiza uma notificação para um usuário.
        /// </summary>
        /// <param name="model">O modelo com os dados da notificação a ser criada ou atualizada.</param>
        /// <returns>Uma tarefa assíncrona que representa a operação de criação ou atualização da notificação.</returns>
        Task PostUserNotification(UserNotificationUpdateModel model);
    }
}

using Microsoft.AspNetCore.Mvc;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using static SoliGest.Server.Controllers.UserNotificationsController;

namespace SoliGest.Server.Services
{
    /// <summary>
    /// Serviço responsável pela criação de notificações para os usuários.
    /// Este serviço gerencia a adição de notificações ao banco de dados.
    /// </summary>
    public class UserNotificationService : IUserNotificationService
    {
        private readonly SoliGestServerContext _context;

        /// <summary>
        /// Construtor da classe UserNotificationService.
        /// Inicializa o contexto do banco de dados para acesso aos dados.
        /// </summary>
        /// <param name="context">O contexto do banco de dados.</param>
        public UserNotificationService(SoliGestServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria uma notificação associada a um usuário e a salva no banco de dados.
        /// </summary>
        /// <param name="model">O modelo contendo os dados da notificação a ser criada.</param>
        /// <returns>Uma tarefa assíncrona representando a operação de adicionar a notificação.</returns>
        /// <exception cref="Exception">Lança exceções caso o usuário ou a notificação não sejam encontrados no banco de dados, ou caso ocorra um erro ao salvar.</exception>
        public async Task PostUserNotification(UserNotificationUpdateModel model)
        {
            try
            {
                // Recupera o usuário com o ID fornecido.
                User user = await _context.FindAsync<User>(model.UserId);
                if (user == null)
                    throw new Exception("O utilizador com o Id fornecido não foi encontrado.");

                // Recupera a notificação com o ID fornecido.
                Notification notification = await _context.FindAsync<Notification>(model.NotificationId);
                if (notification == null)
                    throw new Exception("A notificação com o Id fornecido não foi encontrada.");

                // Cria a associação entre o usuário e a notificação.
                UserNotification userNotification = new UserNotification
                {
                    UserNotificationId = 0, // ID gerado automaticamente pelo banco de dados
                    User = user,
                    UserId = model.UserId,
                    Notification = notification,
                    NotificationId = model.NotificationId,
                    ReceivedDate = DateTime.Now,
                    IsRead = false // Definido como não lido por padrão
                };

                // Adiciona a notificação ao contexto do banco de dados.
                _context.UserNotification.Add(userNotification);

                // Salva as alterações no banco de dados.
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception("Erro.");
            }
        }
    }
}

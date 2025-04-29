using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Models;

namespace SoliGest.Server.Data
{
    /// <summary>
    /// Classe responsável por popular a tabela UserNotification com dados iniciais. 
    /// A classe verifica se já existem notificações associadas aos usuários no banco de dados 
    /// e, se não, cria notificações para o usuário com e-mail "technician@mail.com".
    /// </summary>
    public class UserNotificationSeeder
    {
        /// <summary>
        /// Método assíncrono responsável por semear a tabela UserNotification com dados de exemplo.
        /// Ele verifica se a tabela já contém registros, e caso contrário, associa notificações de exemplo ao usuário.
        /// </summary>
        /// <param name="context">O contexto do banco de dados para acessar as entidades.</param>
        public static async Task SeedUserNotificationsAsync(SoliGestServerContext context)
        {
            // Verifica se já existem notificações para os usuários
            if (!context.UserNotification.Any())
            {
                // Busca o usuário com o e-mail 'technician@mail.com'
                User user = await context.Users.FirstOrDefaultAsync(u => u.Email.Equals("technician@mail.com"));

                // Busca as notificações com IDs específicos
                Notification notification1 = await context.Notification.FindAsync(1);
                Notification notification2 = await context.Notification.FindAsync(2);
                Notification notification3 = await context.Notification.FindAsync(3);

                // Cria a associação de notificações para o usuário
                UserNotification userNotification1 = new UserNotification
                {
                    UserNotificationId = 0,   // Identificador único da notificação do usuário
                    User = user,              // Associa o usuário
                    UserId = user.Id,         // Referência ao ID do usuário
                    Notification = notification1,  // Associa a primeira notificação
                    NotificationId = notification1.Id,  // ID da notificação
                    ReceivedDate = DateTime.Now, // Data de recebimento da notificação
                    IsRead = false            // Status de leitura da notificação
                };

                // Adiciona a primeira associação
                await context.AddAsync(userNotification1);

                // Criação de outras associações
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

                // Adiciona a terceira associação
                await context.AddAsync(userNotification3);

                // Salva as mudanças no banco de dados
                await context.SaveChangesAsync();
            }
        }
    }
}

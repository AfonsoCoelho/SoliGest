using Microsoft.AspNetCore.Identity;
using SoliGest.Server.Models;
using SoliGest.Server.Data;
using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Data
{
    /// <summary>
    /// Classe responsável por popular a tabela de notificações com dados iniciais.
    /// </summary>
    public class NotificationSeeder
    {
        /// <summary>
        /// Método assíncrono para semear dados na tabela de notificações.
        /// Verifica se já existem registros na tabela e, caso não haja, cria notificações iniciais.
        /// </summary>
        /// <param name="context">O contexto do banco de dados que será utilizado para adicionar os registros.</param>
        /// <returns>Uma tarefa assíncrona representando a operação de semear os dados.</returns>
        public static async Task SeedNotificationsAsync(SoliGestServerContext context)
        {
            // Verifica se já existem notificações no banco de dados
            if (!context.Notification.Any())
            {
                // Criação de uma notificação do tipo "message"
                Notification notification1 = new Notification
                {
                    Title = "Nova assistência técnica atribuída!",
                    Type = "message",
                    Message = "Clique para saber mais sobre o painel avariado."
                };

                await context.AddAsync(notification1);

                // Criação de uma notificação do tipo "alert"
                Notification notification2 = new Notification
                {
                    Title = "Notificação 2",
                    Type = "alert",
                    Message = "Assistência técnica 99 solucionada!"
                };

                await context.AddAsync(notification2);

                // Criação de uma notificação do tipo "warning"
                Notification notification3 = new Notification
                {
                    Title = "Notificação 3",
                    Type = "warning",
                    Message = "Uma assistência técnica foi atualizada!"
                };

                await context.AddAsync(notification3);

                // Salva as alterações no banco de dados
                await context.SaveChangesAsync();
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoliGest.Server.Repositories
{
    /// <summary>
    /// Repositório responsável pela gestão de conversas e mensagens no sistema de chat.
    /// </summary>
    public class ChatRepository : IChatRepository
    {
        private readonly SoliGestServerContext _db;

        /// <summary>
        /// Inicializa uma nova instância do repositório de chat.
        /// </summary>
        /// <param name="db">O contexto do banco de dados.</param>
        public ChatRepository(SoliGestServerContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Obtém todas as conversas de um usuário específico.
        /// </summary>
        /// <param name="userId">O ID do usuário cujas conversas são recuperadas.</param>
        /// <returns>Uma coleção de conversas do usuário.</returns>
        public async Task<IEnumerable<Conversation>> GetConversationsFor(string userId)
        {
            return await _db.Conversations
                .Include(c => c.Contact)      // Navegação para o Contact
                .Include(c => c.Messages)     // Navegação para as mensagens
                .Where(c => c.Users.Any(u => u.Id == userId))  // Filtra por usuários participantes
                .ToListAsync();
        }

        /// <summary>
        /// Obtém os contatos disponíveis para um usuário específico.
        /// </summary>
        /// <param name="userId">O ID do usuário cujos contatos são recuperados.</param>
        /// <returns>Uma coleção de contatos disponíveis.</returns>
        public async Task<IEnumerable<Contact>> GetAvailableContacts(string userId)
        {
            return await _db.Users
                .Where(u => u.Id != userId)   // Filtra para excluir o próprio usuário
                .Select(u => new Contact { Id = u.Id, Name = u.Name })  // Seleciona informações do contato
                .ToListAsync();
        }

        /// <summary>
        /// Salva uma nova mensagem ou atualiza uma conversa existente.
        /// </summary>
        /// <param name="senderId">O ID do usuário que envia a mensagem.</param>
        /// <param name="receiverId">O ID do usuário que recebe a mensagem.</param>
        /// <param name="content">O conteúdo da mensagem.</param>
        /// <param name="timestamp">A data e hora em que a mensagem foi enviada.</param>
        /// <returns>Uma tarefa assíncrona que representa a operação.</returns>
        public async Task SaveMessage(string senderId, string receiverId, string content, DateTime timestamp)
        {
            // Tenta obter conversa existente entre sender e receiver
            var conv = await _db.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c =>
                    c.Users.Any(u => u.Id == senderId) &&
                    c.Users.Any(u => u.Id == receiverId));

            if (conv == null)
            {
                // Criar nova conversa
                conv = new Conversation
                {
                    CreatedAt = DateTime.UtcNow,
                    Users = new List<User>
                    {
                        new User { Id = senderId },
                        new User { Id = receiverId }
                    },
                    Messages = new List<Message>()
                };
                _db.Conversations.Add(conv);
            }

            // Adicionar mensagem
            conv.Messages.Add(new Message
            {
                SenderId = senderId,
                Sender = _db.Users.Find(senderId),
                ReceiverId = receiverId,
                Receiver = _db.Users.Find(receiverId),
                Content = content,
                Timestamp = timestamp
            });

            await _db.SaveChangesAsync();
        }
    }
}

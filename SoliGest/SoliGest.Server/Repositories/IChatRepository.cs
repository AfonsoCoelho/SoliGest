using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoliGest.Server.Models;

namespace SoliGest.Server.Repositories
{
    /// <summary>
    /// Interface para o repositório de chat, que define as operações de gerenciamento de conversas e mensagens.
    /// </summary>
    public interface IChatRepository
    {
        /// <summary>
        /// Obtém todas as conversas de um usuário específico.
        /// </summary>
        /// <param name="userId">O ID do usuário cujas conversas serão recuperadas.</param>
        /// <returns>Uma coleção assíncrona de conversas do usuário.</returns>
        Task<IEnumerable<Conversation>> GetConversationsFor(string userId);

        /// <summary>
        /// Obtém os contatos disponíveis para um usuário específico, ou seja, usuários com os quais o usuário não está em uma conversa.
        /// </summary>
        /// <param name="userId">O ID do usuário cujos contatos serão recuperados.</param>
        /// <returns>Uma coleção assíncrona de contatos disponíveis.</returns>
        Task<IEnumerable<Contact>> GetAvailableContacts(string userId);

        /// <summary>
        /// Salva uma nova mensagem ou atualiza uma conversa existente entre dois usuários.
        /// </summary>
        /// <param name="senderId">O ID do usuário que envia a mensagem.</param>
        /// <param name="receiverId">O ID do usuário que recebe a mensagem.</param>
        /// <param name="content">O conteúdo da mensagem.</param>
        /// <param name="timestamp">A data e hora em que a mensagem foi enviada.</param>
        /// <returns>Uma tarefa assíncrona que representa a operação de salvar a mensagem.</returns>
        Task SaveMessage(string senderId, string receiverId, string content, DateTime timestamp);
    }
}

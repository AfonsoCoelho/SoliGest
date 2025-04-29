using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SoliGest.Server.Hubs
{
    /// <summary>
    /// Classe que implementa o hub de chat utilizando SignalR.
    /// Permite que os usuários enviem e recebam mensagens em tempo real.
    /// </summary>
    public class ChatHub : Hub
    {
        /// <summary>
        /// Método responsável por enviar uma mensagem de um usuário para outro.
        /// Envia a mensagem para o usuário de destino com o conteúdo e a data/hora do envio.
        /// </summary>
        /// <param name="receiverId">O identificador do usuário receptor da mensagem.</param>
        /// <param name="content">O conteúdo da mensagem enviada.</param>
        /// <returns>Uma tarefa assíncrona que representa o envio da mensagem.</returns>
        public async Task SendMessage(string receiverId, string content)
        {
            // Obtém o identificador do usuário remetente
            var senderId = Context.UserIdentifier;
            // Marca a data e hora de envio da mensagem (em UTC)
            var timestamp = DateTime.UtcNow;

            // Envia a mensagem para o receptor, incluindo o identificador do remetente, o conteúdo e a data/hora
            await Clients.User(receiverId)
                         .SendAsync("ReceiveMessage", senderId, content, timestamp);
        }
    }
}

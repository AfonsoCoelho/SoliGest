namespace SoliGest.Server.Models
{
    /// <summary>
    /// Representa uma mensagem enviada entre usuários em uma conversa.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Identificador único da mensagem.
        /// </summary>
        /// <remarks>Este campo é utilizado para identificar a mensagem de forma única no sistema.</remarks>
        public int Id { get; set; }

        /// <summary>
        /// Conteúdo da mensagem enviada.
        /// </summary>
        /// <remarks>Este campo contém o texto ou dados enviados pelo remetente na mensagem.</remarks>
        public required string Content { get; set; }

        /// <summary>
        /// Data e hora em que a mensagem foi enviada.
        /// </summary>
        /// <remarks>Este campo registra a data e hora de envio da mensagem.</remarks>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Identificador único do usuário remetente.
        /// </summary>
        /// <remarks>Este campo relaciona a mensagem ao usuário que a enviou.</remarks>
        public required string SenderId { get; set; }

        /// <summary>
        /// Usuário remetente da mensagem.
        /// </summary>
        /// <remarks>Este campo contém o objeto `User` que representa o remetente da mensagem.</remarks>
        public required User Sender { get; set; }

        /// <summary>
        /// Identificador único do usuário destinatário.
        /// </summary>
        /// <remarks>Este campo relaciona a mensagem ao usuário que a recebeu.</remarks>
        public required string ReceiverId { get; set; }

        /// <summary>
        /// Usuário destinatário da mensagem.
        /// </summary>
        /// <remarks>Este campo contém o objeto `User` que representa o destinatário da mensagem.</remarks>
        public required User Receiver { get; set; }

        /// <summary>
        /// Identificador único da conversa à qual a mensagem pertence.
        /// </summary>
        /// <remarks>Este campo faz o vínculo da mensagem a uma conversa específica no sistema.</remarks>
        public int ConversationId { get; set; }

        /// <summary>
        /// A conversa à qual a mensagem pertence.
        /// </summary>
        /// <remarks>Este campo contém o objeto `Conversation` que representa a conversa associada à mensagem.</remarks>
        public Conversation Conversation { get; set; }
    }
}

namespace SoliGest.Server.Models
{
    public class Message
    {
        public int Id { get; set; } // Identificador único da mensagem
        public required string Content { get; set; } // Conteúdo da mensagem
        public DateTime Timestamp { get; set; } // Data e hora da mensagem
        public required string SenderId { get; set; } // ID do remetente
        public required User Sender { get; set; } // Navegação para o remetente
        public required string ReceiverId { get; set; } // ID do remetente
        public required User Receiver { get; set; } // Navegação para o destinatário

        public required int ConversationId { get; set; } // ID da conversa
        public required Conversation Conversation { get; set; } // Navegação para a conversa
    }
}

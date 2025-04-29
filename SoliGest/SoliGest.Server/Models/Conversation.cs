using System;
using System.Collections.Generic;

namespace SoliGest.Server.Models
{
    /// <summary>
    /// Representa uma conversa no sistema, associada a um contato específico e contendo mensagens trocadas entre usuários.
    /// </summary>
    public class Conversation
    {
        /// <summary>
        /// Identificador único da conversa.
        /// </summary>
        /// <remarks>Este campo é utilizado para identificar a conversa de forma única no sistema.</remarks>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do contato associado à conversa.
        /// </summary>
        /// <remarks>Este campo relaciona a conversa a um contato específico no sistema.</remarks>
        public string ContactId { get; set; }

        /// <summary>
        /// Contato associado à conversa.
        /// </summary>
        /// <remarks>Este campo armazena a referência para o contato relacionado à conversa.</remarks>
        public Contact Contact { get; set; }

        /// <summary>
        /// Lista de mensagens trocadas dentro da conversa.
        /// </summary>
        /// <remarks>Este campo contém todas as mensagens associadas à conversa.</remarks>
        public ICollection<Message> Messages { get; set; }

        /// <summary>
        /// Lista de usuários participantes da conversa.
        /// </summary>
        /// <remarks>Este campo armazena os usuários envolvidos na conversa.</remarks>
        public ICollection<User> Users { get; set; }

        /// <summary>
        /// Data e hora em que a conversa foi criada.
        /// </summary>
        /// <remarks>Este campo marca o momento em que a conversa foi iniciada no sistema.</remarks>
        public DateTime CreatedAt { get; set; }
    }
}

namespace SoliGest.Server.Models
{
    /// <summary>
    /// Representa uma notificação que pode ser enviada para os usuários.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Identificador único da notificação.
        /// </summary>
        /// <remarks>Este campo é utilizado para identificar a notificação de forma única no sistema.</remarks>
        public int Id { get; set; }

        /// <summary>
        /// Título da notificação.
        /// </summary>
        /// <remarks>Este campo contém o título ou assunto da notificação, utilizado para chamar a atenção do usuário.</remarks>
        public required string Title { get; set; }

        /// <summary>
        /// Tipo da notificação.
        /// </summary>
        /// <remarks>Este campo define o tipo da notificação, como por exemplo, "Aviso", "Erro", "Informação", etc.</remarks>
        public required string Type { get; set; }

        /// <summary>
        /// Mensagem que será exibida na notificação.
        /// </summary>
        /// <remarks>Este campo contém o conteúdo detalhado da notificação, que será apresentado ao usuário.</remarks>
        public required string Message { get; set; }
    }
}

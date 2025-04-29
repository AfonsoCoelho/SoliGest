namespace SoliGest.Server.Models
{
    /// <summary>
    /// Representa a associação entre um usuário e uma notificação no sistema.
    /// A classe armazena informações sobre quando o usuário recebeu a notificação e se ela foi lida.
    /// </summary>
    public class UserNotification
    {
        /// <summary>
        /// Identificador único da associação entre o usuário e a notificação.
        /// </summary>
        public int UserNotificationId { get; set; }

        /// <summary>
        /// Referência ao usuário associado à notificação.
        /// </summary>
        /// <remarks>Esta propriedade armazena o usuário que recebeu a notificação.</remarks>
        public virtual required User User { get; set; }

        /// <summary>
        /// Identificador do usuário associado à notificação.
        /// </summary>
        /// <remarks>Esta propriedade armazena o ID do usuário associado à notificação.</remarks>
        public required string UserId { get; set; }

        /// <summary>
        /// Referência à notificação associada ao usuário.
        /// </summary>
        /// <remarks>Esta propriedade armazena a notificação que foi recebida pelo usuário.</remarks>
        public virtual required Notification Notification { get; set; }

        /// <summary>
        /// Identificador da notificação associada ao usuário.
        /// </summary>
        /// <remarks>Esta propriedade armazena o ID da notificação associada ao usuário.</remarks>
        public int NotificationId { get; set; }

        /// <summary>
        /// Data e hora em que a notificação foi recebida pelo usuário.
        /// </summary>
        /// <remarks>Este campo armazena a data e hora exatas em que a notificação foi recebida.</remarks>
        public DateTime ReceivedDate { get; set; }

        /// <summary>
        /// Indica se a notificação foi lida pelo usuário.
        /// </summary>
        /// <remarks>Este campo é um valor booleano que especifica se o usuário já leu a notificação.</remarks>
        public bool IsRead { get; set; }

        /// <summary>
        /// Link relacionado à notificação, se existir.
        /// </summary>
        /// <remarks>Este campo pode armazenar um link relacionado à notificação, por exemplo, para redirecionar o usuário a uma página relevante.</remarks>
        public string? Link { get; set; }
    }
}

namespace SoliGest.Server.Services
{
    /// <summary>
    /// Interface para o serviço de envio de emails.
    /// Define a operação de envio de emails para destinatários.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envia um email de forma assíncrona.
        /// </summary>
        /// <param name="toEmail">O endereço de email do destinatário.</param>
        /// <param name="subject">O assunto do email.</param>
        /// <param name="body">O conteúdo do corpo do email.</param>
        /// <returns>Uma tarefa assíncrona que representa a operação de envio de email.</returns>
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}

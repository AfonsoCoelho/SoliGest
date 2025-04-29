using SendGrid;
using SendGrid.Helpers.Mail;

namespace SoliGest.Server.Services
{
    /// <summary>
    /// Implementação do serviço de envio de e-mails utilizando o SendGrid.
    /// Esta classe é responsável por enviar e-mails através da API do SendGrid.
    /// </summary>
    public class SendGridEmailService : IEmailService
    {
        private readonly string _sendGridApiKey;

        /// <summary>
        /// Construtor da classe SendGridEmailService.
        /// Inicializa a chave da API do SendGrid a partir da configuração fornecida.
        /// </summary>
        /// <param name="configuration">A configuração que contém a chave da API do SendGrid.</param>
        public SendGridEmailService(IConfiguration configuration)
        {
            _sendGridApiKey = configuration["SendGrid:ApiKey"];
        }

        /// <summary>
        /// Envia um e-mail assíncrono utilizando o SendGrid.
        /// </summary>
        /// <param name="toEmail">O e-mail do destinatário.</param>
        /// <param name="subject">O assunto do e-mail.</param>
        /// <param name="body">O conteúdo do e-mail (corpo).</param>
        /// <returns>Uma tarefa assíncrona que representa a operação de envio do e-mail.</returns>
        /// <exception cref="Exception">Lança uma exceção caso a chave da API do SendGrid esteja ausente ou se o envio do e-mail falhar.</exception>
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(_sendGridApiKey))
                throw new Exception("SendGrid API Key is missing. Please check your configuration.");

            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress("soligestesa@gmail.com", "SoliGest");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
        }
    }
}

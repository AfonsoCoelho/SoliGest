using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace SoliGestIntegrationTests
{



    /// <summary>
    /// Handler de autenticação personalizado usado para testes.
    /// Este handler simula a autenticação, verificando se o cabeçalho de autorização está presente 
    /// e atribuindo um usuário de teste no processo de autenticação.
    /// </summary>
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /// <summary>
        /// Construtor do TestAuthHandler.
        /// </summary>
        /// <param name="options">As opções de monitoramento da autenticação.</param>
        /// <param name="logger">O logger para registrar informações de autenticação.</param>
        /// <param name="encoder">Codificador de URL usado durante a autenticação.</param>
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder
        ) : base(options, logger, encoder) { }

        /// <summary>
        /// Lida com a autenticação, verificando se o cabeçalho de autorização está presente.
        /// Se estiver, cria um usuário de teste e retorna a autenticação com sucesso.
        /// </summary>
        /// <returns>O resultado da autenticação.</returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Verifica se o cabeçalho de autorização está presente
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.NoResult());

            // Cria as claims do usuário de teste
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "test-user") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            // Cria o ticket de autenticação
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        /// <summary>
        /// Lida com o desafio de autenticação, retornando um status 401 se a autenticação falhar.
        /// </summary>
        /// <param name="properties">As propriedades da autenticação.</param>
        /// <returns>Uma tarefa representando a operação de desafio.</returns>
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
    }
}
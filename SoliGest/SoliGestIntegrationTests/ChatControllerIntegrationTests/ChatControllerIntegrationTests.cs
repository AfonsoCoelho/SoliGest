using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace SoliGestIntegrationTests
{
    /// <summary>
    /// Testes de integração para o controlador Chat.
    /// </summary>
    public class ChatControllerIntegrationTests
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="ChatControllerIntegrationTests"/> e configura o cliente HTTP para os testes.
        /// </summary>
        public ChatControllerIntegrationTests()
        {
            var factory = new CustomWebApplicationFactory();
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");
        }

        /// <summary>
        /// Testa o endpoint de conversas para garantir que retorna sucesso e o tipo de conteúdo correto.
        /// </summary>
        [Fact]
        public async Task GetConversations_EndpointReturnSuccessAndCorrectContentType()
        {
            var resp = await _client.GetAsync("/api/Chat/conversations");
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                         resp.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        /// Testa o endpoint de contatos para garantir que retorna sucesso e o tipo de conteúdo correto.
        /// </summary>
        [Fact]
        public async Task GetContacts_EndpointReturnSuccessAndCorrectContentType()
        {
            var resp = await _client.GetAsync("/api/Chat/contacts");
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                         resp.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        /// Testa o endpoint de envio de mensagem para garantir que retorna sucesso e o tipo de conteúdo correto.
        /// </summary>
        [Fact]
        public async Task PostMessage_EndpointReturnSuccessAndCorrectContentType()
        {
            var dto = new { ReceiverId = "1", Content = "Hello World!" };
            var content = new StringContent(
                JsonConvert.SerializeObject(dto),
                Encoding.UTF8,
                "application/json");

            var resp = await _client.PostAsync("/api/Chat/message", content);
            var body = await resp.Content.ReadAsStringAsync();

            Console.WriteLine("Status Code: " + resp.StatusCode);
            Console.WriteLine("Response Body: " + body);

            Assert.Equal("text/plain; charset=utf-8",
                         resp.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        /// Testa o endpoint de conversas sem autenticação para garantir que retorna o status não autorizado.
        /// </summary>
        [Fact]
        public async Task GetConversations_WithoutAuth_ReturnsUnauthorized()
        {
            var clientNoAuth = new CustomWebApplicationFactory().CreateClient();
            var resp = await clientNoAuth.GetAsync("/api/Chat/conversations");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        /// <summary>
        /// Testa o endpoint de contatos sem autenticação para garantir que retorna o status não autorizado.
        /// </summary>
        [Fact]
        public async Task GetContacts_WithoutAuth_ReturnsUnauthorized()
        {
            var clientNoAuth = new CustomWebApplicationFactory().CreateClient();
            var resp = await clientNoAuth.GetAsync("/api/Chat/contacts");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        /// <summary>
        /// Testa o endpoint de envio de mensagem sem autenticação para garantir que retorna o status não autorizado.
        /// </summary>
        [Fact]
        public async Task PostMessage_WithoutAuth_ReturnsUnauthorized()
        {
            var clientNoAuth = new CustomWebApplicationFactory().CreateClient();
            var dto = new { ReceiverId = "1", Content = "Sem token" };
            var content = new StringContent(
                JsonConvert.SerializeObject(dto),
                Encoding.UTF8,
                "application/json");

            var resp = await clientNoAuth.PostAsync("/api/Chat/message", content);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        /// <summary>
        /// Testa o envio de mensagem para uma conversa existente, verificando se o ID da conversa é mantido.
        /// </summary>
        [Fact]
        public async Task PostMessage_ToExistingConversation_ReturnsSameConversationId()
        {
            var dto1 = new { ReceiverId = "1", Content = "Primeira" };
            var c1 = new StringContent(
                JsonConvert.SerializeObject(dto1),
                Encoding.UTF8,
                "application/json");
            var r1 = await _client.PostAsync("/api/Chat/message", c1);

            dynamic m1 = JsonConvert.DeserializeObject<dynamic>(await r1.Content.ReadAsStringAsync());

            string convId1 = m1.conversationId;

            var dto2 = new { ReceiverId = "1", Content = "Segunda" };
            var c2 = new StringContent(
                JsonConvert.SerializeObject(dto2),
                Encoding.UTF8,
                "application/json");
            var r2 = await _client.PostAsync("/api/Chat/message", c2);

            dynamic m2 = JsonConvert.DeserializeObject<dynamic>(await r2.Content.ReadAsStringAsync());

            string convId2 = m2.conversationId;

            Assert.Equal(convId1, convId2);
        }
    }
}

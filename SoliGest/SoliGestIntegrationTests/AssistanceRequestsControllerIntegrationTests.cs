using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SoliGest.Server.Controllers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestIntegrationTests
{
    /// <summary>
    /// Testes de integração para o controller de pedidos de assistência (AssistanceRequestsController).
    /// Verifica as respostas dos endpoints e o tipo de conteúdo retornado.
    /// </summary>
    public class AssistanceRequestsControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        /// <summary>
        /// Construtor padrão para os testes, inicializa a WebApplicationFactory.
        /// </summary>
        public AssistanceRequestsControllerIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        /// <summary>
        /// Verifica se o endpoint GET /api/AssistanceRequests retorna sucesso e conteúdo JSON.
        /// </summary>
        [Fact]
        public async Task Get_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.GetAsync("/api/AssistanceRequests");

            response.EnsureSuccessStatusCode(); // 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        /// Verifica se o endpoint GET /api/AssistanceRequests/{id} retorna sucesso e conteúdo JSON.
        /// </summary>
        [Fact]
        public async Task GetById_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.GetAsync("/api/AssistanceRequests/2");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        /// Verifica se o endpoint DELETE /api/AssistanceRequests/{id} retorna sucesso e conteúdo JSON.
        /// </summary>
        [Fact]
        public async Task Delete_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.DeleteAsync("/api/AssistanceRequests/1");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        /// Verifica se o endpoint PUT /api/AssistanceRequests/{id} retorna sucesso e conteúdo JSON.
        /// </summary>
        [Fact]
        public async Task Put_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();
            var arum = new AssistanceRequestUpdateModel
            {
                Description = "New description",
                Priority = "New priority",
                RequestDate = new DateOnly().ToString(),
                ResolutionDate = "2025-01-01",
                SolarPanelId = 2,
                Status = "New Status",
                StatusClass = "New Status Class"
            };

            var content = new StringContent(JsonConvert.SerializeObject(arum), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/AssistanceRequests/2", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        /// Verifica se o endpoint POST /api/AssistanceRequests retorna sucesso e conteúdo JSON.
        /// </summary>
        [Fact]
        public async Task Post_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();
            var arcm = new AssistanceRequestCreateModel
            {
                Description = "New description",
                Priority = "New priority",
                RequestDate = new DateOnly().ToString(),
                ResolutionDate = "2025-01-01",
                SolarPanelId = 3,
                Status = "New Status",
                StatusClass = "New Status Class"
            };

            var content = new StringContent(JsonConvert.SerializeObject(arcm), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/AssistanceRequests/", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}

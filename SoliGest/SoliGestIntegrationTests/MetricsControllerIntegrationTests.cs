using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestIntegrationTests
{
    /// <summary>
    /// Testes de integração para o controller de métricas (MetricsController).
    /// Verifica o sucesso das requisições e o tipo de conteúdo das respostas.
    /// </summary>
    public class MetricsControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        /// <summary>
        /// Construtor padrão que inicializa a aplicação para testes.
        /// </summary>
        public MetricsControllerIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        /// <summary>
        /// Verifica se o endpoint GET /api/Metrics/total-usuarios retorna sucesso e o número total de usuários.
        /// </summary>
        [Fact]
        public async Task GetTotalUsers_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Metrics/total-usuarios");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(content);
            Assert.NotNull(result.totalUsers);
        }

        /// <summary>
        /// Verifica se o endpoint GET /api/Metrics/total-paineis retorna sucesso e o número total de painéis solares.
        /// </summary>
        [Fact]
        public async Task GetTotalPainels_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Metrics/total-paineis");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(content);
            Assert.NotNull(result.totalPanels);
        }

        /// <summary>
        /// Verifica se o endpoint GET /api/Metrics/total-pedidos-assistencia retorna sucesso e o número total de pedidos de assistência.
        /// </summary>
        [Fact]
        public async Task GetTotalAssistanceRequests_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Metrics/total-pedidos-assistencia");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(content);
            Assert.NotNull(result.totalAssistanceRequests);
        }

        /// <summary>
        /// Verifica se o endpoint GET /api/Metrics/avarias-priority retorna sucesso e dados sobre a prioridade das avarias.
        /// </summary>
        [Fact]
        public async Task GetAssistanceRequestPerStatus_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Metrics/avarias-priority");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(content);
            Assert.NotNull(result);
        }

        /// <summary>
        /// Verifica se o endpoint GET /api/Metrics/tempo-medio-reparacao retorna sucesso e o tempo médio de reparo.
        /// </summary>
        [Fact]
        public async Task GetAverageRepairTime_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Metrics/tempo-medio-reparacao");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(content);
            Assert.NotNull(result.averageRepairTime);
        }
    }
}

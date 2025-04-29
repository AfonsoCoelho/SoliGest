using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SoliGest.Server.Controllers;
using SoliGest.Server.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestIntegrationTests
{
    /// <summary>
    /// Testes de integração para o controlador SolarPanelsController.
    /// </summary>
    public class SolarPanelsControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        /// <summary>
        /// Construtor da classe de testes, inicializa a fábrica da aplicação.
        /// </summary>
        public SolarPanelsControllerIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        /// <summary>
        /// Cria um painel solar de teste na API e retorna o objeto criado.
        /// </summary>
        /// <param name="client">Cliente HTTP utilizado para comunicar com a API.</param>
        /// <returns>Instância de <see cref="SolarPanel"/> criada na API.</returns>
        private async Task<SolarPanel> CreateTestSolarPanel(HttpClient client)
        {
            var panel = new SolarPanel
            {
                Name = "Painel Teste",
                Priority = "Alta",
                Status = "Ativo",
                StatusClass = "info",
                Latitude = 10.0,
                Longitude = 20.0,
                Description = "Painel de teste",
                PhoneNumber = 123456789,
                Email = "teste@painel.com",
                Address = "Rua Teste"
            };

            var content = new StringContent(JsonConvert.SerializeObject(panel), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/SolarPanels", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SolarPanel>(responseString);
        }

        /// <summary>
        /// Verifica se a chamada GET à rota /api/SolarPanels retorna sucesso e o tipo de conteúdo correto.
        /// </summary>
        [Fact]
        public async Task Get_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();

            var response = await client.GetAsync("/api/SolarPanels");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        /// Verifica se a chamada GET por ID retorna sucesso e o tipo de conteúdo correto.
        /// </summary>
        [Fact]
        public async Task GetById_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();
            var created = await CreateTestSolarPanel(client);

            var response = await client.GetAsync($"/api/SolarPanels/{created.Id}");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        /// Verifica se a chamada DELETE remove o painel e retorna sucesso.
        /// </summary>
        [Fact]
        public async Task Delete_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();
            var created = await CreateTestSolarPanel(client);

            var response = await client.DeleteAsync($"/api/SolarPanels/{created.Id}");

            response.EnsureSuccessStatusCode();
            Assert.Equal(0, response.Content.Headers.ContentLength);
        }

        /// <summary>
        /// Verifica se a chamada PUT atualiza um painel existente e retorna sucesso e o tipo de conteúdo correto.
        /// </summary>
        [Fact]
        public async Task Put_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();
            var created = await CreateTestSolarPanel(client);

            var spum = new SolarPanelUpdateModel
            {
                Id = created.Id,
                Name = "Novo nome",
                Priority = "Nova prioridade",
                Status = "Novo status",
                StatusClass = "nova-class",
                Latitude = 1.1,
                Longitude = 2.2,
                Description = "Descrição atualizada",
                PhoneNumber = 999999999,
                Email = "atualizado@mail.com",
                Address = "Endereço atualizado"
            };

            var content = new StringContent(JsonConvert.SerializeObject(spum), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/SolarPanels/{created.Id}", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        /// Verifica se a chamada POST cria um novo painel e retorna sucesso e o tipo de conteúdo correto.
        /// </summary>
        [Fact]
        public async Task Post_EndpointReturnSuccessAndCorrectContentType()
        {
            HttpClient client = _factory.CreateClient();

            var sp = new SolarPanel
            {
                Name = "Novo Painel",
                Priority = "Média",
                Status = "Inativo",
                StatusClass = "warning",
                Latitude = 12.34,
                Longitude = 56.78,
                Description = "Painel criado via teste",
                PhoneNumber = 987654321,
                Email = "novo@painel.com",
                Address = "Rua Nova"
            };

            var content = new StringContent(JsonConvert.SerializeObject(sp), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/SolarPanels", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
    }
}

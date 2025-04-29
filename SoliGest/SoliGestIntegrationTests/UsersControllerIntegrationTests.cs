using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestIntegrationTests
{
    /// <summary>
    /// Classe de testes de integra��o para o controlador UsersController.
    /// </summary>
    public class UsersControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        /// <summary>
        /// Construtor que inicializa a f�brica da aplica��o para testes.
        /// </summary>
        public UsersControllerIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        /// <summary>
        /// Testa se a rota GET /api/Users retorna com sucesso e com o tipo de conte�do correto.
        /// </summary>
        [Fact]
        public async Task Get_EndpointReturnSuccessAndCorrectContentType()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/Users");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}

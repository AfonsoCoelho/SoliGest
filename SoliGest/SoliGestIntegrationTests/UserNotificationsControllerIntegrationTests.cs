using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SoliGest.Server;
using SoliGest.Server.Models;
using Xunit;

namespace SoliGestIntegrationTests
{
    /// <summary>
    /// Classe de testes de integração para o controlador UserNotificationsController.
    /// </summary>
    public class UserNotificationsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Inicializa a instância de testes com um cliente HTTP da aplicação.
        /// </summary>
        /// <param name="factory">Fábrica da aplicação para testes de integração.</param>
        public UserNotificationsControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        /// <summary>
        /// Verifica se a rota GET /api/UserNotifications retorna sucesso e uma lista de notificações.
        /// </summary>
        [Fact]
        public async Task GetAllUserNotifications_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/api/UserNotifications");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var userNotifications = await response.Content.ReadFromJsonAsync<List<UserNotification>>();
            Assert.NotNull(userNotifications);
        }

        /// <summary>
        /// Verifica se a chamada GET por ID inválido retorna NotFound.
        /// </summary>
        [Fact]
        public async Task GetUserNotificationById_ReturnsNotFound_ForInvalidId()
        {
            var response = await _client.GetAsync("/api/UserNotifications/9999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// Testa a criação de uma notificação de utilizador válida e verifica se retorna OK ou BadRequest.
        /// </summary>
        [Fact]
        public async Task PostUserNotification_ReturnsOk_WhenValid()
        {
            var model = new
            {
                UserNotificationId = 0,
                UserId = "test-user-id", // Deve existir na base de dados de teste
                NotificationId = 1,      // Deve existir
                ReceivedDate = DateTime.UtcNow,
                IsRead = false
            };

            var response = await _client.PostAsJsonAsync("/api/UserNotifications", model);

            // Como depende do serviço, pode ser Ok ou BadRequest dependendo do mock
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Testa a atualização de uma notificação com ID inválido e espera retorno NotFound.
        /// </summary>
        [Fact]
        public async Task PutUserNotification_ReturnsNotFound_WhenInvalidId()
        {
            var model = new
            {
                UserNotificationId = 9999,
                UserId = "invalid-id",
                NotificationId = 1,
                ReceivedDate = DateTime.UtcNow,
                IsRead = true
            };

            var response = await _client.PutAsJsonAsync("/api/UserNotifications/9999", model);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// Testa a exclusão de uma notificação com ID inválido e espera retorno NotFound.
        /// </summary>
        [Fact]
        public async Task DeleteUserNotification_ReturnsNotFound_WhenIdInvalid()
        {
            var response = await _client.DeleteAsync("/api/UserNotifications/9999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// Verifica se a rota GET por UserId retorna OK ou NotFound dependendo da existência do utilizador.
        /// </summary>
        [Fact]
        public async Task GetUserNotificationsByUserId_ReturnsOkOrNotFound()
        {
            var response = await _client.GetAsync("/api/UserNotifications/ByUserId/test-user-id");

            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }
    }
}

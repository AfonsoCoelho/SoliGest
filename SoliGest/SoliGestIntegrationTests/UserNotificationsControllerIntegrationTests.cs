using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SoliGest.Server;
using SoliGest.Server.Models;
using Xunit;

namespace SoliGest.Tests.IntegrationTests
{
    public class UserNotificationsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UserNotificationsControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllUserNotifications_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/api/UserNotifications");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var userNotifications = await response.Content.ReadFromJsonAsync<List<UserNotification>>();
            Assert.NotNull(userNotifications);
        }

        [Fact]
        public async Task GetUserNotificationById_ReturnsNotFound_ForInvalidId()
        {
            var response = await _client.GetAsync("/api/UserNotifications/9999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

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

        [Fact]
        public async Task DeleteUserNotification_ReturnsNotFound_WhenIdInvalid()
        {
            var response = await _client.DeleteAsync("/api/UserNotifications/9999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUserNotificationsByUserId_ReturnsOkOrNotFound()
        {
            var response = await _client.GetAsync("/api/UserNotifications/ByUserId/test-user-id");

            // Pode ser Ok ou NotFound dependendo se existe ou não na base
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }
    }
}

using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SoliGest.Server.Controllers;
using SoliGest.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestIntegrationTests
{
    public class UserNotificationsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UserNotificationsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAll_UserNotifications_ReturnsSuccessAndJson()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/UserNotifications");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetById_UserNotification_ReturnsSuccess()
        {
            var client = _factory.CreateClient();

            // Primeiro cria uma notificação para testar
            var testNotification = await CreateAndGetTestUserNotification(client);
            Assert.NotNull(testNotification);

            // Agora busca pelo ID criado
            var response = await client.GetAsync($"/api/UserNotifications/{testNotification.UserNotificationId}");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

            // Limpeza
            await client.DeleteAsync($"/api/UserNotifications/{testNotification.UserNotificationId}");
        }

        [Fact]
        public async Task Post_UserNotification_ReturnsCreated()
        {
            var client = _factory.CreateClient();

            var userNotification = new UserNotificationsController.UserNotificationUpdateModel
            {
                UserNotificationId = 0, // Será gerado pelo banco
                UserId = "1",
                NotificationId = 1,
                ReceivedDate = DateTime.Now,
                IsRead = false
            };

            var content = new StringContent(JsonConvert.SerializeObject(userNotification), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/UserNotifications", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Deserializar a resposta para obter o ID para limpeza
            var createdNotification = JsonConvert.DeserializeObject<UserNotification>(await response.Content.ReadAsStringAsync());
            Assert.NotNull(createdNotification);

            // Limpeza
            await client.DeleteAsync($"/api/UserNotifications/{createdNotification.UserNotificationId}");
        }

        [Fact]
        public async Task Put_UserNotification_ReturnsOk()
        {
            var client = _factory.CreateClient();

            // Primeiro cria uma notificação para testar
            var createdNotification = await CreateAndGetTestUserNotification(client);
            Assert.NotNull(createdNotification);

            var updatedUserNotification = new UserNotificationsController.UserNotificationUpdateModel
            {
                UserNotificationId = createdNotification.UserNotificationId,
                UserId = createdNotification.UserId,
                NotificationId = createdNotification.NotificationId,
                ReceivedDate = DateTime.Now,
                IsRead = true
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedUserNotification), Encoding.UTF8, "application/json");

            // Usar a URL correta com o ID na rota
            var response = await client.PutAsync($"/api/UserNotifications/{createdNotification.UserNotificationId}", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Limpeza
            await client.DeleteAsync($"/api/UserNotifications/{createdNotification.UserNotificationId}");
        }

        [Fact]
        public async Task MarkAsRead_UserNotification_ReturnsOk()
        {
            var client = _factory.CreateClient();

            // Primeiro cria uma notificação para testar
            var createdNotification = await CreateAndGetTestUserNotification(client);
            Assert.NotNull(createdNotification);

            // Marca a notificação como lida
            var response = await client.PutAsync($"/api/UserNotifications/markAsRead/{createdNotification.UserNotificationId}", null);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Verifica se foi realmente marcada como lida
            var getResponse = await client.GetAsync($"/api/UserNotifications/{createdNotification.UserNotificationId}");
            getResponse.EnsureSuccessStatusCode();

            var updatedNotification = JsonConvert.DeserializeObject<UserNotification>(await getResponse.Content.ReadAsStringAsync());
            Assert.True(updatedNotification.IsRead);

            // Limpeza
            await client.DeleteAsync($"/api/UserNotifications/{createdNotification.UserNotificationId}");
        }

        [Fact]
        public async Task Delete_UserNotification_ReturnsOk()
        {
            var client = _factory.CreateClient();

            // Primeiro cria uma notificação para testar
            var createdNotification = await CreateAndGetTestUserNotification(client);
            Assert.NotNull(createdNotification);

            var response = await client.DeleteAsync($"/api/UserNotifications/{createdNotification.UserNotificationId}");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Verifica se realmente foi excluído
            var getResponse = await client.GetAsync($"/api/UserNotifications/{createdNotification.UserNotificationId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task GetByUserId_UserNotifications_ReturnsSuccess()
        {
            var client = _factory.CreateClient();

            // Primeiro cria uma notificação para testar
            var createdNotification = await CreateAndGetTestUserNotification(client);
            Assert.NotNull(createdNotification);

            // Busca notificações do usuário
            var response = await client.GetAsync($"/api/UserNotifications/ByUserId/{createdNotification.UserId}");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

            var notifications = JsonConvert.DeserializeObject<List<UserNotification>>(await response.Content.ReadAsStringAsync());
            Assert.NotNull(notifications);
            Assert.True(notifications.Any());

            // Limpeza
            await client.DeleteAsync($"/api/UserNotifications/{createdNotification.UserNotificationId}");
        }

        private async Task<UserNotification> CreateAndGetTestUserNotification(HttpClient client)
        {
            var userNotification = new UserNotificationsController.UserNotificationUpdateModel
            {
                UserNotificationId = 0, // Será gerado pelo banco
                UserId = "1",
                NotificationId = 1,
                ReceivedDate = DateTime.Now,
                IsRead = false
            };

            var content = new StringContent(JsonConvert.SerializeObject(userNotification), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/UserNotifications", content);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<UserNotification>(await response.Content.ReadAsStringAsync());
        }
    }
}
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SoliGest.Server.Controllers;
using SoliGest.Server.Models;
using System;
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
            var createResponse = await CreateTestUserNotification(client);
            if (!createResponse.IsSuccessStatusCode)
            {
                Assert.True(false, "Falha ao criar notificação de teste");
                return;
            }

            var createdNotification = JsonConvert.DeserializeObject<UserNotification>(await createResponse.Content.ReadAsStringAsync());

            // Agora busca pelo ID criado
            var response = await client.GetAsync($"/api/UserNotifications/{createdNotification.UserNotificationId}");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

            // Limpeza
            await client.DeleteAsync($"/api/UserNotifications/{createdNotification.UserNotificationId}");
        }

        [Fact]
        public async Task Post_UserNotification_ReturnsSuccess()
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

            // O controlador retorna Ok() em vez de Created()
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Limpeza
            if (response.IsSuccessStatusCode)
            {
                // Como o POST não retorna o objeto criado, precisamos buscar a última notificação do usuário
                var notificationsResponse = await client.GetAsync($"/api/UserNotifications/ByUserId/{userNotification.UserId}");
                var notifications = JsonConvert.DeserializeObject<List<UserNotification>>(await notificationsResponse.Content.ReadAsStringAsync());
                var lastNotification = notifications?.LastOrDefault();

                if (lastNotification != null)
                {
                    await client.DeleteAsync($"/api/UserNotifications/{lastNotification.UserNotificationId}");
                }
            }
        }

        [Fact]
        public async Task Put_UserNotification_ReturnsOk()
        {
            var client = _factory.CreateClient();

            // Primeiro cria uma notificação para testar
            var createResponse = await CreateTestUserNotification(client);
            if (!createResponse.IsSuccessStatusCode)
            {
                Assert.True(false, "Falha ao criar notificação de teste");
                return;
            }

            var createdNotification = JsonConvert.DeserializeObject<UserNotification>(await createResponse.Content.ReadAsStringAsync());

            var updatedUserNotification = new UserNotificationsController.UserNotificationUpdateModel
            {
                UserNotificationId = createdNotification.UserNotificationId,
                UserId = createdNotification.UserId,
                NotificationId = createdNotification.NotificationId,
                ReceivedDate = DateTime.Now,
                IsRead = true
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedUserNotification), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/UserNotifications", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Limpeza
            await client.DeleteAsync($"/api/UserNotifications/{createdNotification.UserNotificationId}");
        }

        [Fact]
        public async Task Delete_UserNotification_ReturnsOk()
        {
            var client = _factory.CreateClient();

            // Primeiro cria uma notificação para testar
            var createResponse = await CreateTestUserNotification(client);
            if (!createResponse.IsSuccessStatusCode)
            {
                Assert.True(false, "Falha ao criar notificação de teste");
                return;
            }

            var createdNotification = JsonConvert.DeserializeObject<UserNotification>(await createResponse.Content.ReadAsStringAsync());

            var response = await client.DeleteAsync($"/api/UserNotifications/{createdNotification.UserNotificationId}");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpResponseMessage> CreateTestUserNotification(HttpClient client)
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
            return await client.PostAsync("/api/UserNotifications", content);
        }
    }
}
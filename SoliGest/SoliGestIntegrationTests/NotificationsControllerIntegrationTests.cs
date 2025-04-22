using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SoliGest.Server.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestIntegrationTests
{
    public class NotificationsControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        public NotificationsControllerIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [Fact]
        public async Task GetAll_Notifications_ReturnsSuccessAndJson()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Notifications");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetById_Notification_ReturnsSuccess()
        {
            // Arrange
            var client = _factory.CreateClient();

            // First create a notification
            var newNotification = new
            {
                Title = "Test Notification",
                Type = "Test",
                Message = "Test Message"
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(newNotification), Encoding.UTF8, "application/json");
            var postResponse = await client.PostAsync("/api/Notifications", postContent);
            postResponse.EnsureSuccessStatusCode();

            var createdContent = await postResponse.Content.ReadAsStringAsync();
            var createdNotification = JsonConvert.DeserializeObject<Notification>(createdContent);

            // Now get it by ID
            var response = await client.GetAsync($"/api/Notifications/{createdNotification.Id}");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_Notification_ReturnsCreated()
        {
            // Arrange
            var client = _factory.CreateClient();
            var notification = new
            {
                Title = "Teste de Notificação",
                Type = "Info",
                Message = "Corpo da mensagem"
            };
            var content = new StringContent(
                JsonConvert.SerializeObject(notification),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await client.PostAsync("/api/Notifications", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert sem lançar exceção, para vermos o corpo
            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Esperava 201 Created mas recebi {(int)response.StatusCode} ({response.StatusCode}).\nCorpo da resposta:\n{responseBody}"
            );

            // Se chegar aqui, valida content‑type
            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString()
            );
        }





        [Fact]
        public async Task Put_Notification_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // First create a notification
            var newNotification = new
            {
                Title = "Original Title",
                Type = "Original Type",
                Message = "Original Message"
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(newNotification), Encoding.UTF8, "application/json");
            var postResponse = await client.PostAsync("/api/Notifications", postContent);
            postResponse.EnsureSuccessStatusCode();

            var createdContent = await postResponse.Content.ReadAsStringAsync();
            var createdNotification = JsonConvert.DeserializeObject<Notification>(createdContent);

            // Now update it
            var updatedNotification = new
            {
                Id = createdNotification.Id,
                Title = "Updated Title",
                Type = "Updated Type",
                Message = "Updated Message"
            };

            var putContent = new StringContent(JsonConvert.SerializeObject(updatedNotification), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/Notifications/{createdNotification.Id}", putContent);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Delete_Notification_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // 1) Cria a notificação com todos os campos corretos
            var newNotification = new
            {
                Title = "Notificação Teste",
                Type = "Info",
                Message = "Para testes"
            };
            var postContent = new StringContent(
                JsonConvert.SerializeObject(newNotification),
                Encoding.UTF8,
                "application/json"
            );

            var postResponse = await client.PostAsync("/api/Notifications", postContent);
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            // Lê o corpo para extrair o Id
            var createdJson = await postResponse.Content.ReadAsStringAsync();
            var created = JsonConvert.DeserializeObject<Notification>(createdJson);
            Assert.NotNull(created);
            Assert.True(created.Id > 0);

            // Act: apaga pelo Id
            var deleteResponse = await client.DeleteAsync($"/api/Notifications/{created.Id}");

            // Assert: apenas 200 OK
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }


    }
}

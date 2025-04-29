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
    /// <summary>
    /// Testes de integração para o controller de notificações (NotificationsController).
    /// Valida operações CRUD e a consistência das respostas HTTP.
    /// </summary>
    public class NotificationsControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        /// <summary>
        /// Inicializa a aplicação para testes com WebApplicationFactory.
        /// </summary>
        public NotificationsControllerIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        /// <summary>
        /// Verifica se o endpoint GET /api/Notifications retorna sucesso e conteúdo JSON.
        /// </summary>
        [Fact]
        public async Task GetAll_Notifications_ReturnsSuccessAndJson()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Notifications");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        /// Testa a criação e recuperação de uma notificação via GET /api/Notifications/{id}.
        /// </summary>
        [Fact]
        public async Task GetById_Notification_ReturnsSuccess()
        {
            var client = _factory.CreateClient();

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

            var response = await client.GetAsync($"/api/Notifications/{createdNotification.Id}");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        /// <summary>
        /// Verifica se o endpoint POST /api/Notifications cria uma notificação com sucesso e retorna HTTP 201.
        /// </summary>
        [Fact]
        public async Task Post_Notification_ReturnsCreated()
        {
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

            var response = await client.PostAsync("/api/Notifications", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Esperava 201 Created mas recebi {(int)response.StatusCode} ({response.StatusCode}).\nCorpo da resposta:\n{responseBody}"
            );

            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString()
            );
        }

        /// <summary>
        /// Testa a atualização de uma notificação com PUT /api/Notifications/{id} e verifica o sucesso da operação.
        /// </summary>
        [Fact]
        public async Task Put_Notification_ReturnsOk()
        {
            var client = _factory.CreateClient();

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

        /// <summary>
        /// Testa a exclusão de uma notificação com DELETE /api/Notifications/{id} e valida o retorno HTTP 200.
        /// </summary>
        [Fact]
        public async Task Delete_Notification_ReturnsOk()
        {
            var client = _factory.CreateClient();

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

            var createdJson = await postResponse.Content.ReadAsStringAsync();
            var created = JsonConvert.DeserializeObject<Notification>(createdJson);
            Assert.NotNull(created);
            Assert.True(created.Id > 0);

            var deleteResponse = await client.DeleteAsync($"/api/Notifications/{created.Id}");

            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }
    }
}

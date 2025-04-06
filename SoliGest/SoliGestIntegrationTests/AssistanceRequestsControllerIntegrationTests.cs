using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SoliGest.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestIntegrationTests
{
    public class AssistanceRequestsControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AssistanceRequestsControllerIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [Fact]
        public async Task Get_EndpointReturnSuccessAndCorrectContentType()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/AssistanceRequests");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetById_EndpointReturnSuccessAndCorrectContentType()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/AssistanceRequests/2");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Delete_EndpointReturnSuccessAndCorrectContentType()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/AssistanceRequests/1");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Put_EndpointReturnSuccessAndCorrectContentType()
        {
            // Arrange
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

            // Act
            var content = new StringContent(JsonConvert.SerializeObject(arum), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/AssistanceRequests/2", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_EndpointReturnSuccessAndCorrectContentType()
        {
            // Arrange
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

            // Act
            var content = new StringContent(JsonConvert.SerializeObject(arcm), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/AssistanceRequests/", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
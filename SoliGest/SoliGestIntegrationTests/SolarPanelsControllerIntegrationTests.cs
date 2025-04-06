using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SoliGest.Server.Controllers;
using SoliGest.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestIntegrationTests
{
    public class SolarPanelsControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        public SolarPanelsControllerIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [Fact]
        public async Task Get_EndpointReturnSuccessAndCorrectContentType()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/SolarPanels");

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
            var response = await client.GetAsync("/api/SolarPanels/1");

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
            //await client.DeleteAsync("/api/AssistanceRequests/1");
            var response = await client.DeleteAsync("/api/SolarPanels/2");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(0, response.Content.Headers.ContentLength);
        }

        [Fact]
        public async Task Put_EndpointReturnSuccessAndCorrectContentType()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var spum = new SolarPanelUpdateModel
            {
                Id = 2,
                Name = "New name",
                Priority = "New priority",
                Status = "New Status",
                StatusClass = "New Status Class",
                Latitude = 1,
                Longitude = 1,
                Description = "New description",
                PhoneNumber = 9,
                Email = "new@mail.com",
                Address = "New address"
            };

            // Act
            var content = new StringContent(JsonConvert.SerializeObject(spum), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/SolarPanels/2", content);

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
            var sp = new SolarPanel
            {
                Id = 0,
                Name = "New name",
                Priority = "New priority",
                Status = "New Status",
                StatusClass = "New Status Class",
                Latitude = 1,
                Longitude = 1,
                Description = "New description",
                PhoneNumber = 9,
                Email = "new@mail.com",
                Address = "New address"
            };

            // Act
            var content = new StringContent(JsonConvert.SerializeObject(sp), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/SolarPanels/", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
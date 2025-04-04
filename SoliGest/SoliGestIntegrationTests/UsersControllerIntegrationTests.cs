using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestIntegrationTests
{
    public class UsersControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UsersControllerIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [Theory]
        [InlineData("/api/Users")]
        [InlineData("/api/AssistanceRequests")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            HttpClient client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
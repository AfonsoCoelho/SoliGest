using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestTest
{
    public class NotificationsControllerTest
    {
        private readonly DbContextOptions<SoliGestServerContext> _options;
        private readonly SoliGestServerContext _context;
        private readonly NotificationsController _controller;

        public NotificationsControllerTest()
        {
            // Usa um nome único de base de dados em memória para isolar cada teste
            var dbName = Guid.NewGuid().ToString();
            _options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new SoliGestServerContext(_options);
            _controller = new NotificationsController(_context);
        }

        [Fact]
        public async Task Create_Notification_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var notification = new Notification
            {
                Message = "message",
                Title = "title",
                Type = "type"
            };

            // Act
            var result = await _controller.PostNotification(notification);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetNotification", createdResult.ActionName);
            var returned = Assert.IsType<Notification>(createdResult.Value);
            Assert.Equal(notification.Message, returned.Message);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenNotificationExists()
        {
            // Arrange
            var notification = new Notification
            {
                Message = "message",
                Title = "title",
                Type = "type"
            };
            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();

            // Act
            var actionResult = await _controller.GetNotification(notification.Id);

            // Assert
            Assert.Equal(notification, actionResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNotificationDoesNotExist()
        {
            // Act
            var actionResult = await _controller.GetNotification(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task Update_Notification_ReturnsOk_WhenNotificationExists()
        {
            // Arrange
            var notification = new Notification
            {
                Message = "message",
                Title = "title",
                Type = "type"
            };
            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();

            var updatedNotification = new Notification
            {
                Id = notification.Id,
                Message = "updated message",
                Title = "updated title",
                Type = "updated type"
            };

            // Act
            var result = await _controller.PutNotification(updatedNotification);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Usa reflection porque a propriedade de objeto anónimo é internal
            var value = okResult.Value;
            var prop = value.GetType().GetProperty("message");
            Assert.NotNull(prop);
            var msg = prop.GetValue(value) as string;
            Assert.Equal("Notificação atualizada com sucesso!", msg);
        }

        [Fact]
        public async Task Update_Notification_ReturnsNotFound_WhenNotificationDoesNotExist()
        {
            // Arrange
            var updatedNotification = new Notification
            {
                Id = 999,
                Message = "updated message",
                Title = "updated title",
                Type = "updated type"
            };

            // Act
            var result = await _controller.PutNotification(updatedNotification);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_Notification_ReturnsOk_WhenNotificationExists()
        {
            // Arrange
            var notification = new Notification
            {
                Message = "message",
                Title = "title",
                Type = "type"
            };
            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteNotification(notification.Id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_Notification_ReturnsNotFound_WhenNotificationDoesNotExist()
        {
            // Act
            var result = await _controller.DeleteNotification(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

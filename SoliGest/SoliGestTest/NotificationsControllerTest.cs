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
    /// <summary>
    /// Classe de teste para o controlador de notificações (NotificationsController).
    /// </summary>
    public class NotificationsControllerTest
    {
        private readonly DbContextOptions<SoliGestServerContext> _options;
        private readonly SoliGestServerContext _context;
        private readonly NotificationsController _controller;

        /// <summary>
        /// Construtor para configurar a base de dados em memória e instâncias do controlador.
        /// </summary>
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

        /// <summary>
        /// Testa a criação de uma notificação. Espera-se que a notificação seja criada e retornada com status 201 (Created).
        /// </summary>
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

        /// <summary>
        /// Testa o método GetNotification quando a notificação existe. Espera-se que retorne a notificação.
        /// </summary>
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

        /// <summary>
        /// Testa o método GetNotification quando a notificação não existe. Espera-se que retorne 404 (Not Found).
        /// </summary>
        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNotificationDoesNotExist()
        {
            // Act
            var actionResult = await _controller.GetNotification(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        /// <summary>
        /// Testa a atualização de uma notificação existente. Espera-se que retorne uma resposta 200 (OK).
        /// </summary>
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

        /// <summary>
        /// Testa a atualização de uma notificação inexistente. Espera-se que retorne 404 (Not Found).
        /// </summary>
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

        /// <summary>
        /// Testa a exclusão de uma notificação existente. Espera-se que retorne uma resposta 200 (OK).
        /// </summary>
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

        /// <summary>
        /// Testa a exclusão de uma notificação inexistente. Espera-se que retorne 404 (Not Found).
        /// </summary>
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

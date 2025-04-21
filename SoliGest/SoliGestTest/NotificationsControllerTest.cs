using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using SoliGest.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoliGestTest
{
    public class NotificationsControllerTest
    {
        private readonly DbContextOptions<SoliGestServerContext> _options;
        private readonly SoliGestServerContext _context;
        private readonly IUserNotificationService _userNotificationService;
        private readonly NotificationsController _controller;

        public NotificationsControllerTest()
        {
            _options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: "SoliGestTestDB_Create")
                .Options;

            _context = new SoliGestServerContext(_options);
            _userNotificationService = new UserNotificationService(_context);
            _controller = new NotificationsController(_context);
        }

        [Fact]
        public async Task Create_Notification_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var notification = new Notification
            {
                Id = 0,
                Message = "message",
                Title = "title",
                Type = "type"
            };

            // Act
            var result = await _controller.PostNotification(notification);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetNotification", createdResult.ActionName);

            var returned = createdResult.Value as Notification;
            Assert.NotNull(returned);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenNotificationExists()
        {
            // Arrange
            var notification = new Notification
            {
                Id = 1,
                Message = "message",
                Title = "title",
                Type = "type"
            };

            _context.Notification.Add(notification);

            // Act
            var result = await _controller.GetNotification(1);

            // Assert
            Assert.Equal(notification, result.Value);
        }



        [Fact]
        public async Task GetById_ReturnsNotFound_WhenRequestDoesNotExist()
        {
            // Act
            var result = await _controller.GetNotification(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Update_Notification_ReturnsOk_WhenNotificationExists()
        {
            // Arrange
            var notification = new Notification
            {
                Id = 1,
                Message = "message",
                Title = "title",
                Type = "type"
            };

            _context.Notification.Add(notification);

            var updatedNotification = new Notification
            {
                Id = 1,
                Message = "updated message",
                Title = "updated title",
                Type = "updated type"
            };

            // Act
            var result = await _controller.PutNotification(updatedNotification);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Update_AssistanceRequest_ReturnsNotFound_WhenRequestDoesNotExist()
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
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_Notification_ReturnsOk_WhenRequestExists()
        {
            // Arrange
            var notification = new Notification
            {
                Id = 0,
                Message = "message",
                Title = "title",
                Type = "type"
            };

            _context.Notification.Add(notification);

            // Act
            var result = await _controller.DeleteNotification(1);

            // Assert
            Assert.IsType<Ok>(result);
        }

        [Fact]
        public async Task Delete_Notification_ReturnsNotFound_WhenRequestDoesNotExist()
        {
            // Act
            var result = await _controller.DeleteNotification(999); // ID que não existe

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
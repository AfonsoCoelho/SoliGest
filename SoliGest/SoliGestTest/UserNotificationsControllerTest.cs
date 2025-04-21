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
using static SoliGest.Server.Controllers.UserNotificationsController;

namespace SoliGestTest
{
    public class UserNotificationsControllerTest
    {
        private readonly DbContextOptions<SoliGestServerContext> _options;
        private readonly SoliGestServerContext _context;
        private readonly IUserNotificationService _userNotificationService;
        private readonly UserNotificationsController _controller;

        public UserNotificationsControllerTest()
        {
            _options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: "SoliGestTestDB_Create")
                .Options;

            _context = new SoliGestServerContext(_options);
            _userNotificationService = new UserNotificationService(_context);
            _controller = new UserNotificationsController(_context, _userNotificationService);
        }

        [Fact]
        public async Task Create_UserNotification_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var userNotification = new UserNotificationUpdateModel
            {
                UserNotificationId = 0,
                UserId = "", // Ir buscar id user
                NotificationId = 1, // Criar notificação
                ReceivedDate = DateTime.Now,
                IsRead = false
            };

            // Act
            var result = await _controller.PostUserNotification(userNotification);

            // Assert
            Assert.IsType<Ok>(result);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenUserNotificationExists()
        {
            // Arrange
            var userNotification = new UserNotification
            {
                UserNotificationId = 0,
                UserId = "", // Ir buscar id user
                User = 
                NotificationId = 1, // Criar notificação
                Notification = 
                ReceivedDate = DateTime.Now,
                IsRead = false,
                Link = ""
            };

            _context.UserNotification.Add(userNotification);

            // Act
            var result = await _controller.GetUserNotification(1);

            // Assert
            Assert.Equal(userNotification, result.Value);
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
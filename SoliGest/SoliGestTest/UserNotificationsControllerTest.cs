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
        private readonly UsersController _usersController;
        private readonly NotificationsController _notificationsController;

        public UserNotificationsControllerTest()
        {
            _options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: "SoliGestTestDB_Create")
                .Options;

            _context = new SoliGestServerContext(_options);
            _userNotificationService = new UserNotificationService(_context);
            _controller = new UserNotificationsController(_context, _userNotificationService);
            _notificationsController = new NotificationsController(_context);
        }

        [Fact]
        public async Task Create_UserNotification_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var userNotification = new UserNotificationUpdateModel
            {
                UserNotificationId = 0,
                UserId = "05ec8bf8-45c0-4dc7-a06f-b43e4550b466", // Ir buscar id user
                NotificationId = 1, // Criar notificação
                ReceivedDate = DateTime.Now,
                IsRead = false
            };

            // Act
            var result = await _controller.PostUserNotification(userNotification);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenUserNotificationExists()
        {
            // Arrange
            var userNotification = new UserNotificationUpdateModel
            {
                UserNotificationId = 0,
                UserId = "05ec8bf8-45c0-4dc7-a06f-b43e4550b466", // Ir buscar id user
                NotificationId = 1, // Criar notificação
                ReceivedDate = DateTime.Now,
                IsRead = false
            };

            await _controller.PostUserNotification(userNotification);

            // Act
            var result = await _controller.GetUserNotification(1);

            // Assert
            Assert.Equal(userNotification.UserNotificationId, result.Value.UserNotificationId);
            Assert.Equal(userNotification.UserId, result.Value.UserId);
            Assert.Equal(userNotification.NotificationId, result.Value.NotificationId);
            Assert.Equal(userNotification.ReceivedDate, result.Value.ReceivedDate);
            Assert.Equal(userNotification.IsRead, result.Value.IsRead);
            Assert.IsType<OkResult>(result);
        }



        [Fact]
        public async Task GetById_ReturnsNotFound_WhenUserNotificationDoesNotExist()
        {
            // Act
            var result = await _controller.GetUserNotification(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Update_UserNotification_ReturnsOk_WhenUserNotificationExists()
        {
            // Arrange
            var userNotification = new UserNotificationUpdateModel
            {
                UserNotificationId = 0,
                UserId = "05ec8bf8-45c0-4dc7-a06f-b43e4550b466", // Ir buscar id user
                NotificationId = 1, // Criar notificação
                ReceivedDate = DateTime.Now,
                IsRead = false
            };

            await _controller.PostUserNotification(userNotification);

            var updatedUserNotification = new UserNotificationUpdateModel
            {
                UserNotificationId = 5,
                UserId = "05ec8bf8-45c0-4dc7-a06f-b43e4550b466", // Ir buscar id user
                NotificationId = 2, // Criar notificação
                ReceivedDate = DateTime.Now,
                IsRead = false
            };

            // Act
            var result = await _controller.PutUserNotification(updatedUserNotification);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Update_UserNotification_ReturnsNotFound_WhenUserNotificationDoesNotExist()
        {
            // Arrange
            var userNotification = new UserNotificationUpdateModel
            {
                UserNotificationId = 0,
                UserId = "05ec8bf8-45c0-4dc7-a06f-b43e4550b466", // Ir buscar id user
                NotificationId = 1, // Criar notificação
                ReceivedDate = DateTime.Now,
                IsRead = false
            };

            // Act
            var result = await _controller.PutUserNotification(userNotification);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_UserNotification_ReturnsOk_WhenUserNotificationExists()
        {
            // Arrange
            var userNotification = new UserNotificationUpdateModel
            {
                UserNotificationId = 0,
                UserId = "05ec8bf8-45c0-4dc7-a06f-b43e4550b466", // Ir buscar id user
                NotificationId = 1, // Criar notificação
                ReceivedDate = DateTime.Now,
                IsRead = false
            };

            await _controller.PostUserNotification(userNotification);

            // Act
            var result = await _controller.DeleteUserNotification(1);

            // Assert
            Assert.IsType<Ok>(result);
        }

        [Fact]
        public async Task Delete_UserNotification_ReturnsNotFound_WhenUserNotificationDoesNotExist()
        {
            // Act
            var result = await _controller.DeleteUserNotification(999); // ID que não existe

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
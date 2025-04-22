using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using SoliGest.Server.Services;
using Xunit;
using static SoliGest.Server.Controllers.UserNotificationsController;

namespace SoliGestTest
{
    public class UserNotificationsControllerTest
    {
        private readonly DbContextOptions<SoliGestServerContext> _options;
        private readonly SoliGestServerContext _context;
        private readonly IUserNotificationService _service;
        private readonly UserNotificationsController _controller;

        public UserNotificationsControllerTest()
        {
            var dbName = Guid.NewGuid().ToString();
            _options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            _context = new SoliGestServerContext(_options);
            _service = new UserNotificationService(_context);
            _controller = new UserNotificationsController(_context, _service);
        }

        private User CreateTechnicianUser()
        {
            return new User
            {
                Email = "technician@mail.com",
                Name = "SoliGest Technician",
                UserName = "technician@mail.com",
                Address1 = "morada1",
                Address2 = "morada2",
                BirthDate = "2001-01-01",
                PhoneNumber = "999999999",
                Role = "Técnico",
                DayOff = "Sábado",
                StartHoliday = "2025-06-01",
                EndHoliday = "2025-07-01",
            };
        }

        [Fact]
        public async Task Create_UserNotification_ReturnsSuccessResult()
        {
            var user = CreateTechnicianUser();
            var notification = new Notification { Id = 1, Title = "Title", Type = "type", Message = "message" };
            _context.Users.Add(user);
            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();

            var model = new UserNotificationUpdateModel
            {
                UserId = user.Id,
                NotificationId = notification.Id,
                ReceivedDate = DateTime.Now,
                IsRead = false
            };

            var result = await _controller.PostUserNotification(model);

            var okResult = Assert.IsType<OkResult>(result); 
        }

        [Fact]
        public async Task GetById_ReturnsUserNotification_WhenExists()
        {
            var user = CreateTechnicianUser();
            var notification = new Notification { Id = 1, Title = "Title", Type = "type", Message = "message" };
            _context.Users.Add(user);
            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();

            var userNotification = new UserNotification
            {
                UserNotificationId = 1,
                UserId = user.Id,
                NotificationId = notification.Id,
                User = user,
                Notification = notification,
                ReceivedDate = DateTime.Now,
                IsRead = false
            };
            _context.UserNotification.Add(userNotification);
            await _context.SaveChangesAsync();

            var result = await _controller.GetUserNotification(userNotification.UserNotificationId);
            var returned = Assert.IsType<UserNotification>(result.Value);
            Assert.Equal(userNotification.UserNotificationId, returned.UserNotificationId);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.GetUserNotification(999);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Update_UserNotification_ReturnsSuccess()
        {
            var user = CreateTechnicianUser();
            var notification = new Notification { Id = 1, Title = "Title", Type = "type", Message = "message" };
            _context.Users.Add(user);
            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();

            var userNotification = new UserNotification
            {
                UserNotificationId = 1,
                UserId = user.Id,
                NotificationId = notification.Id,
                User = user,
                Notification = notification,
                ReceivedDate = DateTime.Now.AddDays(-1),
                IsRead = false
            };
            _context.UserNotification.Add(userNotification);
            await _context.SaveChangesAsync();

            var model = new UserNotificationUpdateModel
            {
                UserNotificationId = userNotification.UserNotificationId,
                UserId = user.Id,
                NotificationId = notification.Id,
                ReceivedDate = DateTime.Now,
                IsRead = true
            };

            var result = await _controller.PutUserNotification(model);

            var okObject = Assert.IsType<OkObjectResult>(result);

            // Usa reflection para extrair a propriedade "message" do objeto anônimo
            var messageProp = okObject.Value?.GetType().GetProperty("message");
            var message = messageProp?.GetValue(okObject.Value)?.ToString();

            Assert.Equal("Notificação atualizada com sucesso!", message);
        }


        [Fact]
        public async Task Update_UserNotification_ReturnsNotFound_WhenNotExists()
        {
            var model = new UserNotificationUpdateModel
            {
                UserNotificationId = 999,
                UserId = Guid.NewGuid().ToString(),
                NotificationId = 1,
                ReceivedDate = DateTime.Now,
                IsRead = true
            };

            var result = await _controller.PutUserNotification(model);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_UserNotification_ReturnsOk()
        {
            var user = CreateTechnicianUser();
            var notification = new Notification { Id = 1, Title = "Title", Type = "type", Message = "message" };
            _context.Users.Add(user);
            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();

            var userNotification = new UserNotification
            {
                UserNotificationId = 1,
                UserId = user.Id,
                NotificationId = notification.Id,
                User = user,
                Notification = notification,
                ReceivedDate = DateTime.Now,
                IsRead = false
            };
            _context.UserNotification.Add(userNotification);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteUserNotification(userNotification.UserNotificationId);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_UserNotification_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.DeleteUserNotification(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

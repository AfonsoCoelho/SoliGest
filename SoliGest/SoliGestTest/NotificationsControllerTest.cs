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
            var panel = new SolarPanel
            {
                Id = 4,
                Name = "Painel Teste",
                Description = "Painel de testes unitários",
                Email = "teste@painel.com",
                Priority = "Alta",
                Status = "Ativo",
                StatusClass = "bg-success",
                Address = "Rua Afonso do Teste Correia"
            };

            using (var context = new SoliGestServerContext(_options))
            {
                context.SolarPanel.Add(panel);
                await context.SaveChangesAsync();
            }

            var controller = new AssistanceRequestsController(new SoliGestServerContext(_options), _userNotificationService);

            var updateModel = new AssistanceRequestUpdateModel
            {
                RequestDate = "2025-04-06T08:00:00",
                ResolutionDate = "2025-04-07T12:00:00",
                Description = "Falha elétrica no painel solar atualizado",
                SolarPanelId = 4,
                Priority = "Alta",
                Status = "Fechado",
                StatusClass = "bg-success"
            };

            // Act
            var result = await controller.Update(999, updateModel); // ID que não existe

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            // Obter o valor do resultado
            var messageValue = notFoundResult.Value as string; // O valor deve ser uma string com a mensagem

            // Verifique a mensagem
            Assert.Equal("Não foi possível encontrar a assistência técnica com o ID '999'.", messageValue);
        }

        [Fact]
        public async Task Delete_AssistanceRequest_ReturnsOk_WhenRequestExists()
        {
            // Arrange
            var panel = new SolarPanel
            {
                Id = 5,
                Name = "Painel Teste",
                Description = "Painel de testes unitários",
                Email = "teste@painel.com",
                Priority = "Alta",
                Status = "Ativo",
                StatusClass = "bg-success",
                Address = "Rua Afonso do Teste Correia"
            };

            using (var context = new SoliGestServerContext(_options))
            {
                context.SolarPanel.Add(panel);
                await context.SaveChangesAsync();

                var request = new AssistanceRequest
                {
                    Id = 5,
                    RequestDate = "2025-04-06T08:00:00",
                    ResolutionDate = "2025-04-07T12:00:00",
                    Description = "Falha elétrica no painel solar",
                    SolarPanel = panel,
                    Priority = "Alta",
                    Status = "Aberto",
                    StatusClass = "bg-warning"
                };

                context.AssistanceRequest.Add(request);
                await context.SaveChangesAsync();
            }

            var controller = new AssistanceRequestsController(new SoliGestServerContext(_options), _userNotificationService);

            // Act
            var result = await controller.Delete(5); // ID da solicitação a ser excluída

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value; // Obter o valor retornado

            // Verifique se o valor contém a propriedade 'message'
            var messageProp = response.GetType().GetProperty("message");
            Assert.NotNull(messageProp); // A propriedade deve existir

            var messageValue = messageProp.GetValue(response)?.ToString();
            Assert.Equal("Pedido de assistência removido com sucesso.", messageValue); // Verifique a mensagem
        }

        [Fact]
        public async Task Delete_AssistanceRequest_ReturnsNotFound_WhenRequestDoesNotExist()
        {
            // Arrange
            var panel = new SolarPanel
            {
                Id = 6,
                Name = "Painel Teste",
                Description = "Painel de testes unitários",
                Email = "teste@painel.com",
                Priority = "Alta",
                Status = "Ativo",
                StatusClass = "bg-success",
                Address = "Rua Afonso do Teste Correia"
            };

            using (var context = new SoliGestServerContext(_options))
            {
                context.SolarPanel.Add(panel);
                await context.SaveChangesAsync();
            }

            var controller = new AssistanceRequestsController(new SoliGestServerContext(_options), _userNotificationService);

            // Act
            var result = await controller.Delete(999); // ID que não existe

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = notFoundResult.Value; // Obter o valor retornado

            // Verifique se o valor contém a propriedade 'message'
            var messageProp = response.GetType().GetProperty("message");
            Assert.NotNull(messageProp); // A propriedade deve existir

            var messageValue = messageProp.GetValue(response)?.ToString(); // Obtém o valor da propriedade
            Assert.Equal("Pedido de assistência não encontrado.", messageValue); // Verifique a mensagem
        }
    }
}
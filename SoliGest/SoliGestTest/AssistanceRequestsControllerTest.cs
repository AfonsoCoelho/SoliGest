using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using System.Threading.Tasks;
using SoliGest.Server.Services;

namespace SoliGestTest
{
    /// <summary>
    /// Classe de testes para o controlador <see cref="AssistanceRequestsController"/>.
    /// </summary>
    public class AssistanceRequestsControllerTest
    {
        private readonly DbContextOptions<SoliGestServerContext> _options;
        private readonly SoliGestServerContext _context;
        private readonly IUserNotificationService _userNotificationService;

        /// <summary>
        /// Construtor que configura o contexto do banco de dados em memória e o serviço de notificações de usuários.
        /// </summary>
        public AssistanceRequestsControllerTest()
        {
            _options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: "SoliGestTestDB_Create")
                .Options;

            _context = new SoliGestServerContext(_options);
            _userNotificationService = new UserNotificationService(_context);
        }

        /// <summary>
        /// Testa se o método Create do controlador <see cref="AssistanceRequestsController"/> retorna 
        /// um <see cref="CreatedAtActionResult"/> ao criar uma solicitação de assistência.
        /// </summary>
        [Fact]
        public async Task Create_AssistanceRequest_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var panel = new SolarPanel
            {
                Id = 1,
                Name = "Painel Teste",
                Description = "Painel de testes unitários",
                Email = "teste@painel.com",
                Priority = "Alta",
                Status = "Ativo",
                StatusClass = "bg-success",
                Address = "Rua afonso do teste correia"
            };

            _context.SolarPanel.Add(panel);
            await _context.SaveChangesAsync();

            var controller = new AssistanceRequestsController(_context, _userNotificationService);

            var requestModel = new AssistanceRequestCreateModel
            {
                RequestDate = "2025-04-06T08:00:00",
                ResolutionDate = "2025-04-07T12:00:00",
                Description = "Falha elétrica no painel solar",
                SolarPanelId = 1,
                Priority = "Alta",
                Status = "Aberto",
                StatusClass = "bg-warning"
            };

            // Act
            var result = await controller.Create(requestModel);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetById", createdResult.ActionName);

            var returned = createdResult.Value as AssistanceRequestCreateModel;
            Assert.NotNull(returned);
            Assert.Equal("Falha elétrica no painel solar", returned.Description);
        }

        /// <summary>
        /// Testa se o método Create retorna um <see cref="BadRequestObjectResult"/> 
        /// quando o painel solar não for encontrado no banco de dados.
        /// </summary>
        [Fact]
        public async Task Create_AssistanceRequest_ReturnsBadRequest_WhenSolarPanelNotFound()
        {
            // Arrange
            var controller = new AssistanceRequestsController(_context, _userNotificationService);

            var invalidRequestModel = new AssistanceRequestCreateModel
            {
                RequestDate = "2025-04-06T08:00:00",
                ResolutionDate = "2025-04-07T12:00:00",
                Description = "Falha no painel que não existe",
                SolarPanelId = 999,
                Priority = "Alta",
                Status = "Aberto",
                StatusClass = "bg-warning"
            };

            // Act
            var result = await controller.Create(invalidRequestModel);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequest.Value;

            var messageProp = response.GetType().GetProperty("message");
            var messageValue = messageProp?.GetValue(response)?.ToString();

            Assert.Equal("Painel solar não encontrado.", messageValue);
        }

        /// <summary>
        /// Testa se o método GetById retorna um <see cref="OkObjectResult"/> 
        /// quando a solicitação de assistência existe no banco de dados.
        /// </summary>
        [Fact]
        public async Task GetById_ReturnsOk_WhenRequestExists()
        {
            // Arrange
            var panel = new SolarPanel
            {
                Id = 2,
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
                    Id = 2,
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
            var result = await controller.GetById(2);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AssistanceRequest>>(result);

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            var returnedRequest = Assert.IsType<AssistanceRequest>(okResult.Value);

            Assert.Equal("Falha elétrica no painel solar", returnedRequest.Description);
        }

        /// <summary>
        /// Testa se o método GetById retorna um <see cref="NotFoundObjectResult"/> 
        /// quando a solicitação de assistência não existe.
        /// </summary>
        [Fact]
        public async Task GetById_ReturnsNotFound_WhenRequestDoesNotExist()
        {
            // Arrange
            var controller = new AssistanceRequestsController(new SoliGestServerContext(_options), _userNotificationService);

            // Act
            var result = await controller.GetById(999); // ID que não existe

            // Assert
            var actionResult = Assert.IsType<ActionResult<AssistanceRequest>>(result);
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);

            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            var response = notFoundResult.Value;

            var messageProp = response.GetType().GetProperty("message");
            var messageValue = messageProp?.GetValue(response)?.ToString();

            Assert.Equal("Pedido de assistência não encontrado.", messageValue);
        }

        /// <summary>
        /// Testa se o método Update retorna um <see cref="OkObjectResult"/> 
        /// quando a solicitação de assistência é atualizada com sucesso.
        /// </summary>
        [Fact]
        public async Task Update_AssistanceRequest_ReturnsOk_WhenRequestExists()
        {
            // Arrange
            var panel = new SolarPanel
            {
                Id = 3,
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
                    Id = 3,
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

            var updateModel = new AssistanceRequestUpdateModel
            {
                RequestDate = "2025-04-06T08:00:00",
                ResolutionDate = "2025-04-07T12:00:00",
                Description = "Falha elétrica no painel solar atualizado",
                SolarPanelId = 3,
                Priority = "Alta",
                Status = "Fechado",
                StatusClass = "bg-success"
            };

            // Act
            var result = await controller.Update(3, updateModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseMessage = okResult.Value;

            var messageProp = responseMessage.GetType().GetProperty("message");
            Assert.NotNull(messageProp);

            var messageValue = messageProp.GetValue(responseMessage).ToString();
            Assert.Equal("Pedido de assistência atualizado com sucesso.", messageValue);
        }

        /// <summary>
        /// Testa se o método Update retorna um <see cref="NotFoundObjectResult"/> 
        /// quando a solicitação de assistência não é encontrada.
        /// </summary>
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

            var response = notFoundResult.Value;
            var messageProp = response.GetType().GetProperty("message");
            var messageValue = messageProp?.GetValue(response)?.ToString();

            Assert.Equal("Pedido de assistência não encontrado.", messageValue);
        }
    }
}

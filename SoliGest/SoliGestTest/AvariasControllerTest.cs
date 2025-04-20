//using Xunit;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SoliGest.Server.Controllers;
//using SoliGest.Server.Data;
//using SoliGest.Server.Models;
//using System.Threading.Tasks;

//namespace SoliGestTest
//{
//    public class AvariasControllerTest
//    {
//        private readonly DbContextOptions<SoliGestServerContext> _options;
//        private readonly SoliGestServerContext _context;

//        public AvariasControllerTest()
//        {
//            _options = new DbContextOptionsBuilder<SoliGestServerContext>()
//                .UseInMemoryDatabase(databaseName: "SoliGestTestDB_Create")
//                .Options;

//            _context = new SoliGestServerContext(_options);
//        }

//        [Fact]
//        public async Task Create_AssistanceRequest_ReturnsCreatedAtActionResult()
//        {
//            // Arrange
//            var panel = new SolarPanel
//            {
//                Id = 1,
//                Name = "Painel Teste",
//                Description = "Painel de testes unitários",
//                Email = "teste@painel.com",
//                Priority = "Alta",
//                Status = "Ativo",
//                StatusClass = "bg-success",
//                Address = "Rua afonso do teste correia"
//            };

//            _context.SolarPanel.Add(panel);
//            await _context.SaveChangesAsync();

//            var controller = new AssistanceRequestsController(_context);

//            var requestModel = new AssistanceRequestCreateModel
//            {
//                RequestDate = "2025-04-06T08:00:00",
//                ResolutionDate = "2025-04-07T12:00:00",
//                Description = "Falha elétrica no painel solar",
//                SolarPanelId = 1,
//                Priority = "Alta",
//                Status = "Aberto",
//                StatusClass = "bg-warning"
//            };

//            // Act
//            var result = await controller.Create(requestModel);

//            // Assert
//            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
//            Assert.Equal("GetById", createdResult.ActionName);

//            var returned = createdResult.Value as AssistanceRequestCreateModel;
//            Assert.NotNull(returned);
//            Assert.Equal("Falha elétrica no painel solar", returned.Description);
//        }

//        [Fact]
//        public async Task Create_AssistanceRequest_ReturnsBadRequest_WhenSolarPanelNotFound()
//        {
//            // Arrange
//            var controller = new AssistanceRequestsController(_context);

//            var invalidRequestModel = new AssistanceRequestCreateModel
//            {
//                RequestDate = "2025-04-06T08:00:00",
//                ResolutionDate = "2025-04-07T12:00:00",
//                Description = "Falha no painel que não existe",
//                SolarPanelId = 999,
//                Priority = "Alta",
//                Status = "Aberto",
//                StatusClass = "bg-warning"
//            };

//            // Act
//            var result = await controller.Create(invalidRequestModel);

//            // Assert
//            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
//            var response = badRequest.Value;

//            var messageProp = response.GetType().GetProperty("message");
//            var messageValue = messageProp?.GetValue(response)?.ToString();

//            Assert.Equal("Painel solar não encontrado.", messageValue);
//        }

//        [Fact]
//        public async Task GetById_ReturnsOk_WhenRequestExists()
//        {
//            // Arrange
//            var panel = new SolarPanel
//            {
//                Id = 2,
//                Name = "Painel Teste",
//                Description = "Painel de testes unitários",
//                Email = "teste@painel.com",
//                Priority = "Alta",
//                Status = "Ativo",
//                StatusClass = "bg-success",
//                Address = "Rua Afonso do Teste Correia"
//            };

//            using (var context = new SoliGestServerContext(_options))
//            {
//                context.SolarPanel.Add(panel);
//                await context.SaveChangesAsync();

//                var request = new AssistanceRequest
//                {
//                    Id = 2,
//                    RequestDate = "2025-04-06T08:00:00",
//                    ResolutionDate = "2025-04-07T12:00:00",
//                    Description = "Falha elétrica no painel solar",
//                    SolarPanel = panel,
//                    Priority = "Alta",
//                    Status = "Aberto",
//                    StatusClass = "bg-warning"
//                };

//                context.AssistanceRequest.Add(request);
//                await context.SaveChangesAsync();
//            }

//            var controller = new AssistanceRequestsController(new SoliGestServerContext(_options));

//            // Act
//            var result = await controller.GetById(2);

//            // Assert
//            // Verifique se o resultado é ActionResult<AssistanceRequest>
//            var actionResult = Assert.IsType<ActionResult<AssistanceRequest>>(result);

//            // Acesse o valor e verifique se é um OkObjectResult
//            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

//            // Verifique se o valor é do tipo AssistanceRequest
//            var returnedRequest = Assert.IsType<AssistanceRequest>(okResult.Value);

//            // Verifique a descrição
//            Assert.Equal("Falha elétrica no painel solar", returnedRequest.Description);
//        }



//        [Fact]
//        public async Task GetById_ReturnsNotFound_WhenRequestDoesNotExist()
//        {
//            // Arrange
//            var controller = new AssistanceRequestsController(new SoliGestServerContext(_options));

//            // Act
//            var result = await controller.GetById(999); // ID que não existe

//            // Assert
//            // Alterando para verificar se o resultado é ActionResult<AssistanceRequest>
//            var actionResult = Assert.IsType<ActionResult<AssistanceRequest>>(result);
//            // Verificando se o resultado é NotFound
//            Assert.IsType<NotFoundObjectResult>(actionResult.Result);

//            // Obter o valor da resposta
//            var notFoundResult = actionResult.Result as NotFoundObjectResult;
//            var response = notFoundResult.Value;

//            // Verificar se a mensagem está correta
//            var messageProp = response.GetType().GetProperty("message");
//            var messageValue = messageProp?.GetValue(response)?.ToString();

//            Assert.Equal("Pedido de assistência não encontrado.", messageValue);
//        }

//        [Fact]
//        public async Task Update_AssistanceRequest_ReturnsOk_WhenRequestExists()
//        {
//            // Arrange
//            var panel = new SolarPanel
//            {
//                Id = 3,
//                Name = "Painel Teste",
//                Description = "Painel de testes unitários",
//                Email = "teste@painel.com",
//                Priority = "Alta",
//                Status = "Ativo",
//                StatusClass = "bg-success",
//                Address = "Rua Afonso do Teste Correia"
//            };

//            using (var context = new SoliGestServerContext(_options))
//            {
//                context.SolarPanel.Add(panel);
//                await context.SaveChangesAsync();

//                var request = new AssistanceRequest
//                {
//                    Id = 3,
//                    RequestDate = "2025-04-06T08:00:00",
//                    ResolutionDate = "2025-04-07T12:00:00",
//                    Description = "Falha elétrica no painel solar",
//                    SolarPanel = panel,
//                    Priority = "Alta",
//                    Status = "Aberto",
//                    StatusClass = "bg-warning"
//                };

//                context.AssistanceRequest.Add(request);
//                await context.SaveChangesAsync();
//            }

//            var controller = new AssistanceRequestsController(new SoliGestServerContext(_options));

//            var updateModel = new AssistanceRequestUpdateModel
//            {
//                RequestDate = "2025-04-06T08:00:00",
//                ResolutionDate = "2025-04-07T12:00:00",
//                Description = "Falha elétrica no painel solar atualizado",
//                SolarPanelId = 3,
//                Priority = "Alta",
//                Status = "Fechado",
//                StatusClass = "bg-success"
//            };

//            // Act
//            var result = await controller.Update(3, updateModel); // Atualiza a solicitação com ID 1

//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            var responseMessage = okResult.Value; // Obtenha o valor

//            // Verifique se o valor contém a propriedade 'message'
//            var messageProp = responseMessage.GetType().GetProperty("message");
//            Assert.NotNull(messageProp); // A propriedade deve existir

//            var messageValue = messageProp.GetValue(responseMessage).ToString();
//            Assert.Equal("Pedido de assistência atualizado com sucesso.", messageValue);
//        }

//        [Fact]
//        public async Task Update_AssistanceRequest_ReturnsNotFound_WhenRequestDoesNotExist()
//        {
//            // Arrange
//            var panel = new SolarPanel
//            {
//                Id = 4,
//                Name = "Painel Teste",
//                Description = "Painel de testes unitários",
//                Email = "teste@painel.com",
//                Priority = "Alta",
//                Status = "Ativo",
//                StatusClass = "bg-success",
//                Address = "Rua Afonso do Teste Correia"
//            };

//            using (var context = new SoliGestServerContext(_options))
//            {
//                context.SolarPanel.Add(panel);
//                await context.SaveChangesAsync();
//            }

//            var controller = new AssistanceRequestsController(new SoliGestServerContext(_options));

//            var updateModel = new AssistanceRequestUpdateModel
//            {
//                RequestDate = "2025-04-06T08:00:00",
//                ResolutionDate = "2025-04-07T12:00:00",
//                Description = "Falha elétrica no painel solar atualizado",
//                SolarPanelId = 4,
//                Priority = "Alta",
//                Status = "Fechado",
//                StatusClass = "bg-success"
//            };

//            // Act
//            var result = await controller.Update(999, updateModel); // ID que não existe

//            // Assert
//            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

//            // Obter o valor do resultado
//            var messageValue = notFoundResult.Value as string; // O valor deve ser uma string com a mensagem

//            // Verifique a mensagem
//            Assert.Equal("Não foi possível encontrar a assistência técnica com o ID '999'.", messageValue);
//        }

//        [Fact]
//        public async Task Delete_AssistanceRequest_ReturnsOk_WhenRequestExists()
//        {
//            // Arrange
//            var panel = new SolarPanel
//            {
//                Id = 5,
//                Name = "Painel Teste",
//                Description = "Painel de testes unitários",
//                Email = "teste@painel.com",
//                Priority = "Alta",
//                Status = "Ativo",
//                StatusClass = "bg-success",
//                Address = "Rua Afonso do Teste Correia"
//            };

//            using (var context = new SoliGestServerContext(_options))
//            {
//                context.SolarPanel.Add(panel);
//                await context.SaveChangesAsync();

//                var request = new AssistanceRequest
//                {
//                    Id = 5,
//                    RequestDate = "2025-04-06T08:00:00",
//                    ResolutionDate = "2025-04-07T12:00:00",
//                    Description = "Falha elétrica no painel solar",
//                    SolarPanel = panel,
//                    Priority = "Alta",
//                    Status = "Aberto",
//                    StatusClass = "bg-warning"
//                };

//                context.AssistanceRequest.Add(request);
//                await context.SaveChangesAsync();
//            }

//            var controller = new AssistanceRequestsController(new SoliGestServerContext(_options));

//            // Act
//            var result = await controller.Delete(5); // ID da solicitação a ser excluída

//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            var response = okResult.Value; // Obter o valor retornado

//            // Verifique se o valor contém a propriedade 'message'
//            var messageProp = response.GetType().GetProperty("message");
//            Assert.NotNull(messageProp); // A propriedade deve existir

//            var messageValue = messageProp.GetValue(response)?.ToString();
//            Assert.Equal("Pedido de assistência removido com sucesso.", messageValue); // Verifique a mensagem
//        }

//        [Fact]
//        public async Task Delete_AssistanceRequest_ReturnsNotFound_WhenRequestDoesNotExist()
//        {
//            // Arrange
//            var panel = new SolarPanel
//            {
//                Id = 6,
//                Name = "Painel Teste",
//                Description = "Painel de testes unitários",
//                Email = "teste@painel.com",
//                Priority = "Alta",
//                Status = "Ativo",
//                StatusClass = "bg-success",
//                Address = "Rua Afonso do Teste Correia"
//            };

//            using (var context = new SoliGestServerContext(_options))
//            {
//                context.SolarPanel.Add(panel);
//                await context.SaveChangesAsync();
//            }

//            var controller = new AssistanceRequestsController(new SoliGestServerContext(_options));

//            // Act
//            var result = await controller.Delete(999); // ID que não existe

//            // Assert
//            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
//            var response = notFoundResult.Value; // Obter o valor retornado

//            // Verifique se o valor contém a propriedade 'message'
//            var messageProp = response.GetType().GetProperty("message");
//            Assert.NotNull(messageProp); // A propriedade deve existir

//            var messageValue = messageProp.GetValue(response)?.ToString(); // Obtém o valor da propriedade
//            Assert.Equal("Pedido de assistência não encontrado.", messageValue); // Verifique a mensagem
//        }
//    }
//}

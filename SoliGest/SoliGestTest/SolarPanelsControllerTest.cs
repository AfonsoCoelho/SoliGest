using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestTest
{
    /// <summary>
    /// Contexto que lança exceção no SaveChangesAsync para simular erro.
    /// </summary>
    public class ExceptionSoliGestServerContext : SoliGestServerContext
    {
        public ExceptionSoliGestServerContext(DbContextOptions<SoliGestServerContext> options)
            : base(options) { }

        /// <summary>
        /// Método sobrescrito para simular uma exceção ao salvar mudanças no banco de dados.
        /// </summary>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new Exception("Simulated Exception");
        }
    }

    /// <summary>
    /// Contexto que simula falha ao salvar (retornando 0) no SaveChanges para o PUT.
    /// </summary>
    public class FailSaveSoliGestServerContext : SoliGestServerContext
    {
        public FailSaveSoliGestServerContext(DbContextOptions<SoliGestServerContext> options)
            : base(options) { }

        /// <summary>
        /// Método sobrescrito para simular falha ao salvar, retornando 0.
        /// </summary>
        public override int SaveChanges()
        {
            return 0;
        }
    }

    /// <summary>
    /// Testes para o controlador SolarPanelsController.
    /// </summary>
    public class SolarPanelsControllerTest
    {
        /// <summary>
        /// Helper para criar um contexto InMemory com nome único para isolar os testes.
        /// </summary>
        private static SoliGestServerContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SoliGestServerContext(options);
        }

        #region GetSolarPanel Tests

        /// <summary>
        /// Teste que verifica se o método GetSolarPanel retorna todos os painéis solares.
        /// </summary>
        [Fact]
        public async Task GetSolarPanel_ReturnsAllPanels()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context);

            var panel1 = new SolarPanel
            {
                Id = 1,
                Name = "Panel 1",
                Email = "test1@example.com",
                PhoneNumber = 123456789,
                Address = "Address 1",
                Description = "Description 1",
                Priority = "High",
                Status = "Active",
                StatusClass = "status-class-1"
            };

            var panel2 = new SolarPanel
            {
                Id = 2,
                Name = "Panel 2",
                Email = "test2@example.com",
                PhoneNumber = 987654321,
                Address = "Address 2",
                Description = "Description 2",
                Priority = "Medium",
                Status = "Active",
                StatusClass = "status-class-2"
            };

            context.SolarPanel.Add(panel1);
            context.SolarPanel.Add(panel2);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetSolarPanel();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<SolarPanel>>>(result);
            var returnValue = Assert.IsType<List<SolarPanel>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.Equal("Panel 1", returnValue[0].Name);
            Assert.Equal("Panel 2", returnValue[1].Name);
        }

        /// <summary>
        /// Teste que verifica se o método GetSolarPanel retorna um painel solar pelo Id.
        /// </summary>
        [Fact]
        public async Task GetSolarPanel_ById_ReturnsPanel()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context);

            var panel = new SolarPanel
            {
                Id = 1,
                Name = "Test Panel",
                Email = "test@example.com",
                Address = "Address Test",
                Description = "Some description",
                Priority = "High",
                Status = "Active",
                StatusClass = "status-class"
            };

            context.SolarPanel.Add(panel);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetSolarPanel(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SolarPanel>>(result);
            Assert.Equal(panel.Id, actionResult.Value.Id);
        }

        /// <summary>
        /// Teste que verifica se o método GetSolarPanel retorna NotFound quando o painel não existe.
        /// </summary>
        [Fact]
        public async Task GetSolarPanel_ById_ReturnsNotFound_WhenPanelDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context);

            // Act
            var result = await controller.GetSolarPanel(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SolarPanel>>(result);
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        #endregion

        #region PostSolarPanel Tests

        /// <summary>
        /// Teste que verifica se o método PostSolarPanel cria um novo painel solar.
        /// </summary>
        [Fact]
        public async Task PostSolarPanel_CreatesNewPanel()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context);
            var panel = new SolarPanel
            {
                Name = "New Panel",
                Address = "A",
                Description = "A",
                Email = "test@mail.com",
                Latitude = 1,
                Longitude = 1,
                PhoneNumber = 1,
                Priority = "A",
                Status = "A",
                StatusClass = "A"
            };

            // Act
            var result = await controller.PostSolarPanel(panel);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetSolarPanel", createdAtActionResult.ActionName);
            var createdPanel = Assert.IsType<SolarPanel>(createdAtActionResult.Value);
            Assert.Equal(panel.Name, createdPanel.Name);
        }

        /// <summary>
        /// Teste que verifica se o método PostSolarPanel retorna BadRequest quando ocorre uma exceção.
        /// </summary>
        [Fact]
        public async Task PostSolarPanel_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange: usamos um contexto que lança exceção no SaveChangesAsync
            var options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var exceptionContext = new ExceptionSoliGestServerContext(options);
            var controller = new SolarPanelsController(exceptionContext);
            var panel = new SolarPanel { Id = 1, Name = "New Panel" };

            // Act
            var result = await controller.PostSolarPanel(panel);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        #endregion

        #region PutSolarPanel Tests

        /// <summary>
        /// Teste que verifica se o método PutSolarPanel atualiza um painel solar existente.
        /// </summary>
        [Fact]
        public async Task PutSolarPanel_UpdatesExistingPanel()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context);

            var existingPanel = new SolarPanel
            {
                Id = 1,
                Name = "Rua D. Afonso Henriques, Lisboa",
                Priority = "Alta",
                Status = "Vermelho",
                StatusClass = "status-red",
                Latitude = 38.7223,
                Longitude = -9.1393,
                Description = "Painel próximo ao centro",
                PhoneNumber = 8,
                Email = "contato@empresa.com",
                Address = "a"
            };

            context.SolarPanel.Add(existingPanel);
            await context.SaveChangesAsync();

            var model = new SolarPanelUpdateModel
            {
                Id = 1,
                Name = "New Name",
                Email = "new@email.com",
                Address = "New Address",
                Description = "New Description",
                Latitude = 10.0,
                Longitude = 20.0,
                PhoneNumber = 123456789,
                Priority = "High",
                Status = "Active",
                StatusClass = "success"
            };

            // Act
            var result = await controller.PutSolarPanel(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Painel solar atualizado com sucesso!", okResult.Value.ToString());

            var updatedPanel = await context.SolarPanel.FindAsync(1);
            Assert.Equal(model.Name, updatedPanel.Name);
            Assert.Equal(model.Email, updatedPanel.Email);
        }

        /// <summary>
        /// Teste que verifica se o método PutSolarPanel retorna NotFound quando o painel não existe.
        /// </summary>
        [Fact]
        public async Task PutSolarPanel_ReturnsNotFound_WhenPanelDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context);
            var model = new SolarPanelUpdateModel { Id = 1 };

            // Act
            var result = await controller.PutSolarPanel(model);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        /// <summary>
        /// Teste que verifica se o método PutSolarPanel retorna BadRequest quando a operação de salvar falha.
        /// </summary>
        [Fact]
        public async Task PutSolarPanel_ReturnsBadRequest_WhenSaveFails()
        {
            // Arrange: usamos um contexto que simula falha no SaveChanges (retornando 0)
            var options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var failContext = new FailSaveSoliGestServerContext(options);
            var controller = new SolarPanelsController(failContext);

            var panel = new SolarPanel
            {
                Id = 1,
                Name = "Panel",
                Email = "test@example.com",
                Address = "Address",
                Description = "Description",
                Priority = "High",
                Status = "Active",
                StatusClass = "status-class"
            };

            failContext.SolarPanel.Add(panel);
            await failContext.SaveChangesAsync();

            var model = new SolarPanelUpdateModel { Id = 1, Name = "Updated Name" };

            // Act
            var result = await controller.PutSolarPanel(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region DeleteSolarPanel Tests

        /// <summary>
        /// Teste que verifica se o método DeleteSolarPanel remove um painel solar.
        /// </summary>
        [Fact]
        public async Task DeleteSolarPanel_RemovesPanel()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context);

            var panel = new SolarPanel
            {
                Id = 1,
                Name = "Panel to delete",
                Email = "test@example.com",
                Address = "Address",
                Description = "Description",
                Priority = "High",
                Status = "Active",
                StatusClass = "status-class"
            };

            context.SolarPanel.Add(panel);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.DeleteSolarPanel(1);

            // Assert
            Assert.IsType<OkResult>(result);
            var deletedPanel = await context.SolarPanel.FindAsync(1);
            Assert.Null(deletedPanel);
        }


        ///// <summary>
        ///// Teste que verifica se o método DeleteSolarPanel retorna NotFound quando o painel não existe.
        ///// </summary>
        //[Fact]
        //public async Task DeleteSolarPanel_ReturnsNotFound_WhenPanelDoesNotExist()
        //{
        //    // Arrange
        //    using var context = CreateContext();
        //    var controller = new SolarPanelsController(context);

        //    // Act
        //    var result = await controller.DeleteSolarPanel(1);

        //    // Assert
        //    Assert.IsType<NotFoundObjectResult>(result);
        //}

        #endregion
    }
}

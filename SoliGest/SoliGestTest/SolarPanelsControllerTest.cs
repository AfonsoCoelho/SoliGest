using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using SoliGest.Server.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestTest
{
    public class FakeGeoCodingService : IGeoCodingService
    {
        public Task<GeocodeResult?> GeocodeAsync(string address)
        {
            return Task.FromResult<GeocodeResult?>(new GeocodeResult
            {
                Latitude = 12.34,
                Longitude = 56.78
            });
        }
    }

    // Contexto que lança exceção no SaveChangesAsync para simular erro
    public class ExceptionSoliGestServerContext : SoliGestServerContext
    {
        public ExceptionSoliGestServerContext(DbContextOptions<SoliGestServerContext> options)
            : base(options) { }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new Exception("Simulated Exception");
        }
    }

    // Contexto que simula falha ao salvar (retornando 0) no SaveChanges para o PUT
    public class FailSaveSoliGestServerContext : SoliGestServerContext
    {
        public FailSaveSoliGestServerContext(DbContextOptions<SoliGestServerContext> options)
            : base(options) { }

        public override int SaveChanges()
        {
            return 0;
        }
    }

    public class SolarPanelsControllerTest
    {
        // Helper para criar um contexto InMemory com nome único para isolar os testes
        private static SoliGestServerContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new SoliGestServerContext(options);
        }

        #region GetSolarPanel Tests

        [Fact]
        public async Task GetSolarPanel_ReturnsAllPanels()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context, new FakeGeoCodingService());

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

        [Fact]
        public async Task GetSolarPanel_ById_ReturnsPanel()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context, new FakeGeoCodingService());

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

        [Fact]
        public async Task GetSolarPanel_ById_ReturnsNotFound_WhenPanelDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context, new FakeGeoCodingService());

            // Act
            var result = await controller.GetSolarPanel(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SolarPanel>>(result);
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        #endregion

        #region PostSolarPanel Tests

        [Fact]
        public async Task PostSolarPanel_CreatesNewPanel()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context, new FakeGeoCodingService());
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

        [Fact]
        public async Task PostSolarPanel_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange: usamos um contexto que lança exceção no SaveChangesAsync
            var options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var exceptionContext = new ExceptionSoliGestServerContext(options);
            var controller = new SolarPanelsController(exceptionContext, new FakeGeoCodingService());
            var panel = new SolarPanel { Id = 1, Name = "New Panel" };

            // Act
            var result = await controller.PostSolarPanel(panel);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        #endregion

        #region PutSolarPanel Tests

        [Fact]
        public async Task PutSolarPanel_UpdatesExistingPanel()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context, new FakeGeoCodingService());

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

        [Fact]
        public async Task PutSolarPanel_ReturnsNotFound_WhenPanelDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context, new FakeGeoCodingService());
            var model = new SolarPanelUpdateModel { Id = 1 };

            // Act
            var result = await controller.PutSolarPanel(model);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PutSolarPanel_ReturnsBadRequest_WhenSaveFails()
        {
            // Arrange: usamos um contexto que simula falha no SaveChanges (retornando 0)
            var options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var failContext = new FailSaveSoliGestServerContext(options);
            var controller = new SolarPanelsController(failContext, new FakeGeoCodingService());

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

        [Fact]
        public async Task DeleteSolarPanel_RemovesPanel()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context, new FakeGeoCodingService());

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

        [Fact]
        public async Task DeleteSolarPanel_ReturnsNotFound_WhenPanelDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new SolarPanelsController(context, new FakeGeoCodingService());

            // Act
            var result = await controller.DeleteSolarPanel(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteSolarPanel_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange: usamos um contexto que lança exceção no SaveChangesAsync
            var options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var exceptionContext = new ExceptionSoliGestServerContext(options);
            var controller = new SolarPanelsController(exceptionContext, new FakeGeoCodingService());
            var panel = new SolarPanel { Id = 1, Name = "Panel" };
            exceptionContext.SolarPanel.Add(panel);
            await exceptionContext.SaveChangesAsync(); // O método Add funciona normalmente.

            // Act
            var result = await controller.DeleteSolarPanel(1);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        #endregion
    }
}

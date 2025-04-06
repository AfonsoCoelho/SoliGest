using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace SoliGestTest
{
    public class SolarPanelsControllerTest
    {
        private readonly Mock<SoliGestServerContext> _mockContext;
        private readonly SolarPanelsController _controller;

        public SolarPanelsControllerTest()
        {
            var options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var context = new SoliGestServerContext(options);

            _controller = new SolarPanelsController(context);
        }

        [Fact]
        public async Task GetSolarPanel_ReturnsAllPanels()
        {
            // Arrange
            var testData = new List<SolarPanel>
            {
                new SolarPanel { Id = 1, Name = "Panel 1", Email = "test1@example.com", PhoneNumber = 123456789, Address = "Address 1" },
                new SolarPanel { Id = 2, Name = "Panel 2", Email = "test2@example.com", PhoneNumber = 987654321, Address = "Address 2" }
            }.AsQueryable();

            // Create mock DbSet
            var mockDbSet = new Mock<DbSet<SolarPanel>>();

            // Setup IQueryable implementation
            mockDbSet.As<IQueryable<SolarPanel>>().Setup(m => m.Provider).Returns(testData.Provider);
            mockDbSet.As<IQueryable<SolarPanel>>().Setup(m => m.Expression).Returns(testData.Expression);
            mockDbSet.As<IQueryable<SolarPanel>>().Setup(m => m.ElementType).Returns(testData.ElementType);
            mockDbSet.As<IQueryable<SolarPanel>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());

            // Setup async operations
            mockDbSet.As<IAsyncEnumerable<SolarPanel>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<SolarPanel>(testData.GetEnumerator()));

            // Create mock context
            var mockContext = new Mock<SoliGestServerContext>(new DbContextOptionsBuilder<SoliGestServerContext>().Options);
            mockContext.Setup(c => c.Set<SolarPanel>()).Returns(mockDbSet.Object);

            var controller = new SolarPanelsController(mockContext.Object);

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
            var panel = new SolarPanel { Id = 1, Name = "Test Panel" };
            _mockContext.Setup(c => c.FindAsync<SolarPanel>(1)).ReturnsAsync(panel);

            // Act
            var result = await _controller.GetSolarPanel(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SolarPanel>>(result);
            Assert.Equal(panel, actionResult.Value);
        }

        [Fact]
        public async Task GetSolarPanel_ById_ReturnsNotFound_WhenPanelDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(c => c.FindAsync<SolarPanel>(1)).ReturnsAsync((SolarPanel)null);

            // Act
            var result = await _controller.GetSolarPanel(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SolarPanel>>(result);
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostSolarPanel_CreatesNewPanel()
        {
            // Arrange
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
            var result = await _controller.PostSolarPanel(panel);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetSolarPanel", createdAtActionResult.ActionName);
            Assert.Equal(panel, createdAtActionResult.Value);
        }

        [Fact]
        public async Task PostSolarPanel_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var panel = new SolarPanel { Id = 1, Name = "New Panel" };
            var mockSet = new Mock<DbSet<SolarPanel>>();

            var mockContext = new Mock<SoliGestServerContext>(new DbContextOptionsBuilder<SoliGestServerContext>().Options);
            mockContext.Setup(c => c.Set<SolarPanel>()).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Throws(new Exception());

            var controller = new SolarPanelsController(mockContext.Object);

            // Act
            var result = await controller.PostSolarPanel(panel);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutSolarPanel_UpdatesExistingPanel()
        {
            // Arrange
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

            var mockContext = new Mock<SoliGestServerContext>(new DbContextOptionsBuilder<SoliGestServerContext>().Options);
            await _controller.PostSolarPanel(existingPanel);
            mockContext.Setup(c => c.FindAsync<SolarPanel>(1)).ReturnsAsync(existingPanel);
            mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = await _controller.PutSolarPanel(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Painel solar atualizado com sucesso!", okResult.Value.ToString());
            Assert.Equal(model.Name, existingPanel.Name);
            Assert.Equal(model.Email, existingPanel.Email);
        }

        [Fact]
        public async Task PutSolarPanel_ReturnsNotFound_WhenPanelDoesNotExist()
        {
            // Arrange
            var model = new SolarPanelUpdateModel { Id = 1 };
            _mockContext.Setup(c => c.FindAsync<SolarPanel>(1)).ReturnsAsync((SolarPanel)null);

            // Act
            var result = await _controller.PutSolarPanel(model);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PutSolarPanel_ReturnsBadRequest_WhenSaveFails()
        {
            // Arrange
            var panel = new SolarPanel { Id = 1 };
            var model = new SolarPanelUpdateModel { Id = 1 };

            _mockContext.Setup(c => c.FindAsync<SolarPanel>(1)).ReturnsAsync(panel);
            _mockContext.Setup(c => c.SaveChanges()).Returns(0);

            // Act
            var result = await _controller.PutSolarPanel(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteSolarPanel_RemovesPanel()
        {
            // Arrange
            var panel = new SolarPanel { Id = 1, Name = "Panel to delete" };
            _mockContext.Setup(c => c.FindAsync<SolarPanel>(1)).ReturnsAsync(panel);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteSolarPanel(1);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockContext.Verify(m => m.Remove(panel), Times.Once());
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task DeleteSolarPanel_ReturnsNotFound_WhenPanelDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(c => c.FindAsync<SolarPanel>(1)).ReturnsAsync((SolarPanel)null);

            // Act
            var result = await _controller.DeleteSolarPanel(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteSolarPanel_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var panel = new SolarPanel { Id = 1 };
            _mockContext.Setup(c => c.FindAsync<SolarPanel>(1)).ReturnsAsync(panel);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Throws(new Exception());

            // Act
            var result = await _controller.DeleteSolarPanel(1);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }

    // Helper classes for async testing
    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }

        public T Current => _inner.Current;
    }

    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(
                    name: nameof(IQueryProvider.Execute),
                    genericParameterCount: 1,
                    types: new[] { typeof(Expression) })
                .MakeGenericMethod(expectedResultType)
                .Invoke(this, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                .MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { executionResult });
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }
}
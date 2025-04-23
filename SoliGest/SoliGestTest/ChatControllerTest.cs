using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using SoliGest.Server;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Hubs;
using SoliGest.Server.Models;
using SoliGest.Server.Repositories;
using Xunit;

namespace SoliGestTest
{
    public class ChatControllerTests
    {
        private readonly Mock<IChatRepository> _repoMock;
        private readonly Mock<IHubContext<ChatHub>> _hubContextMock;
        private readonly Mock<IClientProxy> _clientProxyMock;
        private readonly ChatController _controller;

        public ChatControllerTests()
        {
            _repoMock = new Mock<IChatRepository>();
            _hubContextMock = new Mock<IHubContext<ChatHub>>();
            _clientProxyMock = new Mock<IClientProxy>();

            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(c => c.User(It.IsAny<string>())).Returns(_clientProxyMock.Object);
            _hubContextMock.SetupGet(h => h.Clients).Returns(clientsMock.Object);

            _controller = new ChatController(_hubContextMock.Object, _repoMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", "user1") }, "TestAuth"));
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        }

        [Fact]
        public async Task GetConversations_ReturnsOkObjectResult_WithConversations()
        {
            // Arrange
            var conv = new Conversation { Id = 1, ContactId = "user2", Users = new List<User> { new User { Id = "user1" }, new User { Id = "user2" } }, Messages = new List<Message>() };
            _repoMock.Setup(r => r.GetConversationsFor("user1")).ReturnsAsync(new[] { conv });

            // Act
            var result = await _controller.GetConversationsFor("user1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(new[] { conv });
        }

        [Fact]
        public async Task SendMessage_SavesMessage_AndInvokesHub()
        {
            // Arrange
            var dto = new ChatMessageDto { ReceiverId = "user2", Content = "Olá!" };
            DateTime capturedTime = default;

            _repoMock
                .Setup(r => r.SaveMessage("user1", dto.ReceiverId, dto.Content, It.IsAny<DateTime>()))
                .Callback<string, string, string, DateTime>((s, r, c, t) => capturedTime = t)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SendMessage(dto);

            // Assert
            result.Should().BeOfType<OkResult>();

            _repoMock.Verify(r =>
                r.SaveMessage("user1", dto.ReceiverId, dto.Content, capturedTime),
                Times.Once
            );

            _clientProxyMock.Verify(c =>
                c.SendCoreAsync(
                    "ReceiveMessage",
                    It.Is<object[]>(args =>
                        (string)args[0] == "user1" &&
                        (string)args[1] == dto.Content &&
                        (DateTime)args[2] == capturedTime
                    ),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetConversations_ReturnsUnauthorized_WhenUserIdNotFound()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = await _controller.GetConversationsFor("");

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task SendMessage_ReturnsUnauthorized_WhenUserIdNotFound()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            var dto = new ChatMessageDto { ReceiverId = "user2", Content = "Test" };

            // Act
            var result = await _controller.SendMessage(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }
    }

    // testes do ChatRepository (in-memory database)
    public class ChatRepositoryTests
    {
        private readonly SoliGestServerContext _context;
        private readonly ChatRepository _repo;

        public ChatRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SoliGestServerContext>()
                          .UseInMemoryDatabase(Guid.NewGuid().ToString())
                          .Options;
            _context = new SoliGestServerContext(options);
            _repo = new ChatRepository(_context);
        }

        private User CreateUser(string id)
        {
            return new User
            {
                Id = id,
                UserName = id,
                Name = "Name",
                Email = $"{id}@test.com",
                BirthDate = "2000-01-01",
                Address1 = string.Empty,
                Address2 = string.Empty,
                DayOff = string.Empty,
                StartHoliday = string.Empty,
                EndHoliday = string.Empty,
                Role = string.Empty,
                isActive = true
            };
        }

        [Fact]
        public async Task GetConversationsFor_ReturnsUserConversations()
        {
            // Preparação: usuários e conversa
            var u1 = CreateUser("user1");
            var u2 = CreateUser("user2");
            _context.Users.AddRange(u1, u2);
            foreach (var entry in _context.ChangeTracker.Entries<User>().ToList())
                entry.State = EntityState.Detached;


            var conv = new Conversation { Users = new List<User> { u1, u2 }, Messages = new List<Message>(), ContactId = "user2" };
            _context.Conversations.Add(conv);
            _context.SaveChanges();

            // Act
            var result = await _repo.GetConversationsFor("user1");

            // Assert
            result.Should().ContainSingle();
            result.First().Id.Should().Be(conv.Id);
        }

        [Fact]
        public async Task GetAvailableContacts_ReturnsAllContactsExceptCurrentUser()
        {
            // Preparação: usuários
            var current = CreateUser("u0");
            var contacts = new[] { CreateUser("u1"), CreateUser("u2"), CreateUser("u3") };
            _context.Users.Add(current);
            _context.Users.AddRange(contacts);
            _context.SaveChanges();

            // Act
            var result = await _repo.GetAvailableContacts("u0");

            // Assert
            result.Select(c => c.Id).Should().BeEquivalentTo(new[] { "u1", "u2", "u3" });
        }

        [Fact]
        public async Task SaveMessage_CreatesNewConversation_WhenNotExists()
        {
            // Preparação: apenas usuários
            var u1 = CreateUser("u1");
            var u2 = CreateUser("u2");
            _context.Users.AddRange(u1, u2);
            _context.SaveChanges();

            foreach (var entry in _context.ChangeTracker.Entries<User>().ToList())
                entry.State = EntityState.Detached;

            var content = "Hello";
            var timestamp = DateTime.UtcNow;

            // Act
            await _repo.SaveMessage("u1", "u2", content, timestamp);

            // Assert
            _context.Conversations.Should().ContainSingle();
            var conv = _context.Conversations.Include(c => c.Messages).Single();
            conv.Messages.Should().ContainSingle(m => m.Content == content && m.Timestamp == timestamp);
        }

        [Fact]
        public async Task SaveMessage_AddsToExistingConversation_WhenExists()
        {
            // Preparação: conversa existente
            var u1 = CreateUser("u1");
            var u2 = CreateUser("u2");
            _context.Users.AddRange(u1, u2);
            var conv = new Conversation { Users = new List<User> { u1, u2 }, Messages = new List<Message>(), ContactId = "user2" };
            _context.Conversations.Add(conv);
            _context.SaveChanges();

            foreach (var entry in _context.ChangeTracker.Entries<User>().ToList())
                entry.State = EntityState.Detached;


            var content = "Reply";
            var timestamp = DateTime.UtcNow;

            // Act
            await _repo.SaveMessage("u2", "u1", content, timestamp);

            // Assert
            var msgs = _context.Messages.Where(m => m.ConversationId == conv.Id);
            msgs.Should().Contain(m => m.Content == content && m.Timestamp == timestamp);
        }

        [Fact]
        public async Task SaveMessage_FindsCorrectConversation_WhenUserIdAndContactIdAreReversed()
        {
            // Preparação: conversa com IDs invertidos
            var u1 = CreateUser("u1");
            var u2 = CreateUser("u2");
            _context.Users.AddRange(u1, u2);
            var conv = new Conversation { Users = new List<User> { u2, u1 }, Messages = new List<Message>(), ContactId = "user2" };
            _context.Conversations.Add(conv);
            _context.SaveChanges();

            foreach (var entry in _context.ChangeTracker.Entries<User>().ToList())
                entry.State = EntityState.Detached;

            var content = "Inverse";
            var timestamp = DateTime.UtcNow;

            // Act
            await _repo.SaveMessage("u1", "u2", content, timestamp);

            // Assert
            var msgs = _context.Messages.Where(m => m.ConversationId == conv.Id);
            msgs.Should().Contain(m => m.Content == content && m.Timestamp == timestamp);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using Xunit;

namespace SoliGestTest
{
    public class ChatControllerTest
    {
        private readonly DbContextOptions<SoliGestServerContext> _options;
        private readonly SoliGestServerContext _context;
        private readonly ChatController _controller;
        private readonly string _testUserId = "user-123";

        public ChatControllerTest()
        {
            // Base de dados isolada para cada teste
            var dbName = Guid.NewGuid().ToString();
            _options = new DbContextOptionsBuilder<SoliGestServerContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new SoliGestServerContext(_options);
            _controller = new ChatController(_context);

            // Configurar o controller com HttpContext mockado para simular autenticação
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        // Helper para criar um usuário com propriedades obrigatórias
        private User CreateDummyUser(string id, string name)
        {
            return new User
            {
                Id = id,
                Name = name,
                Address1 = "Address 1",
                Address2 = "Address 2",
                BirthDate = "1990-01-01",
                DayOff = "Monday",
                StartHoliday = "2025-01-01",
                EndHoliday = "2025-01-15",
                Role = "Employee"
            };
        }

        [Fact]
        public async Task GetConversations_ReturnsUserConversations()
        {
            // Arranjar
            var currentUser = CreateDummyUser(_testUserId, "Current User");
            var contact1 = CreateDummyUser("contact-1", "Contact One");
            var contact2 = CreateDummyUser("contact-2", "Contact Two");

            _context.Users.AddRange(currentUser, contact1, contact2);
            await _context.SaveChangesAsync();

            // Adicionar conversas para o usuário de teste
            var conversation1 = new Conversation
            {
                UserId = _testUserId,
                User = currentUser,
                ContactId = contact1.Id,
                Contact = contact1,
                Messages = new List<Message>()
            };

            var conversation2 = new Conversation
            {
                UserId = _testUserId,
                User = currentUser,
                ContactId = contact2.Id,
                Contact = contact2,
                Messages = new List<Message>()
            };

            _context.Conversations.AddRange(conversation1, conversation2);
            await _context.SaveChangesAsync();

            // Agora que temos os IDs das conversas, adicionamos as mensagens
            var message1 = new Message
            {
                SenderId = _testUserId,
                Sender = currentUser,
                ReceiverId = contact1.Id,
                Receiver = contact1,
                Content = "Hello Contact 1",
                Timestamp = DateTime.UtcNow.AddMinutes(-10),
                ConversationId = conversation1.Id,
                Conversation = conversation1
            };

            var message2 = new Message
            {
                SenderId = contact1.Id,
                Sender = contact1,
                ReceiverId = _testUserId,
                Receiver = currentUser,
                Content = "Hi there",
                Timestamp = DateTime.UtcNow.AddMinutes(-5),
                ConversationId = conversation1.Id,
                Conversation = conversation1
            };

            var message3 = new Message
            {
                SenderId = _testUserId,
                Sender = currentUser,
                ReceiverId = contact2.Id,
                Receiver = contact2,
                Content = "Hello Contact 2",
                Timestamp = DateTime.UtcNow.AddMinutes(-8),
                ConversationId = conversation2.Id,
                Conversation = conversation2
            };

            _context.Messages.AddRange(message1, message2, message3);
            await _context.SaveChangesAsync();

            // Agir
            var result = await _controller.GetConversations();

            // Verificar
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var conversations = Assert.IsAssignableFrom<IEnumerable<ConversationDto>>(okResult.Value);
            var conversationsList = conversations.ToList();

            Assert.Equal(2, conversationsList.Count);

            // Em vez de esperar uma ordem específica, verifica cada conversa independentemente
            var conv1 = conversationsList.FirstOrDefault(c => c.Contact.Id == contact1.Id);
            var conv2 = conversationsList.FirstOrDefault(c => c.Contact.Id == contact2.Id);

            Assert.NotNull(conv1);
            Assert.NotNull(conv2);
            Assert.Equal(contact1.Name, conv1.Contact.Name);
            Assert.Equal(2, conv1.Messages.Count);
            Assert.Equal(contact2.Name, conv2.Contact.Name);
            Assert.Equal(1, conv2.Messages.Count);
        }

        [Fact]
        public async Task GetAvailableContacts_ReturnsAllContactsExceptCurrentUser()
        {
            // Arranjar
            var currentUser = CreateDummyUser(_testUserId, "Current User");
            var contact1 = CreateDummyUser("contact-1", "Contact One");
            var contact2 = CreateDummyUser("contact-2", "Contact Two");
            var contact3 = CreateDummyUser("contact-3", "Contact Three");

            _context.Users.AddRange(currentUser, contact1, contact2, contact3);
            await _context.SaveChangesAsync();

            // Agir
            var result = await _controller.GetAvailableContacts();

            // Verificar
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var contacts = Assert.IsAssignableFrom<IEnumerable<ContactDto>>(okResult.Value);
            var contactsList = contacts.ToList();

            Assert.Equal(3, contactsList.Count);
            Assert.DoesNotContain(contactsList, c => c.Id == _testUserId);
            Assert.Contains(contactsList, c => c.Id == contact1.Id);
            Assert.Contains(contactsList, c => c.Id == contact2.Id);
            Assert.Contains(contactsList, c => c.Id == contact3.Id);
        }

        [Fact]
        public async Task SaveMessage_CreatesNewConversation_WhenNotExists()
        {
            // Arranjar
            var currentUser = CreateDummyUser(_testUserId, "Current User");
            var contact = CreateDummyUser("contact-1", "Contact One");

            _context.Users.AddRange(currentUser, contact);
            await _context.SaveChangesAsync();

            var messageDto = new ChatMessageDto
            {
                ReceiverId = contact.Id,
                Content = "New conversation message"
            };

            // Agir
            var result = await _controller.SaveMessage(messageDto);

            // Verificar
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var savedMessage = Assert.IsType<MessageDto>(okResult.Value);

            Assert.Equal(_testUserId, savedMessage.SenderId);
            Assert.Equal(contact.Id, savedMessage.ReceiverId);
            Assert.Equal(messageDto.Content, savedMessage.Content);

            // Verificar se a conversa foi criada
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.UserId == _testUserId && c.ContactId == contact.Id);

            Assert.NotNull(conversation);
            Assert.Single(conversation.Messages);
            Assert.Equal(messageDto.Content, conversation.Messages.First().Content);
        }

        [Fact]
        public async Task SaveMessage_AddsToExistingConversation_WhenExists()
        {
            // Arranjar
            var currentUser = CreateDummyUser(_testUserId, "Current User");
            var contact = CreateDummyUser("contact-1", "Contact One");

            _context.Users.AddRange(currentUser, contact);
            await _context.SaveChangesAsync();

            // Criar uma conversa existente
            var existingConversation = new Conversation
            {
                UserId = _testUserId,
                User = currentUser,
                ContactId = contact.Id,
                Contact = contact,
                Messages = new List<Message>()
            };

            _context.Conversations.Add(existingConversation);
            await _context.SaveChangesAsync();

            // Adicionar a mensagem existente com o ConversationId
            var existingMessage = new Message
            {
                SenderId = _testUserId,
                Sender = currentUser,
                ReceiverId = contact.Id,
                Receiver = contact,
                Content = "Existing message",
                Timestamp = DateTime.UtcNow.AddMinutes(-10),
                ConversationId = existingConversation.Id,
                Conversation = existingConversation
            };

            _context.Messages.Add(existingMessage);
            await _context.SaveChangesAsync();

            var messageDto = new ChatMessageDto
            {
                ReceiverId = contact.Id,
                Content = "New message in existing conversation"
            };

            // Agir
            var result = await _controller.SaveMessage(messageDto);

            // Verificar
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var savedMessage = Assert.IsType<MessageDto>(okResult.Value);

            Assert.Equal(_testUserId, savedMessage.SenderId);
            Assert.Equal(contact.Id, savedMessage.ReceiverId);
            Assert.Equal(messageDto.Content, savedMessage.Content);

            // Verificar se a mensagem foi adicionada à conversa existente
            var updatedConversation = await _context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == existingConversation.Id);

            Assert.NotNull(updatedConversation);
            Assert.Equal(2, updatedConversation.Messages.Count);
            Assert.Contains(updatedConversation.Messages, m => m.Content == messageDto.Content);
        }

        [Fact]
        public async Task SaveMessage_FindsCorrectConversation_WhenUserIdAndContactIdAreReversed()
        {
            // Arranjar
            var currentUser = CreateDummyUser(_testUserId, "Current User");
            var contact = CreateDummyUser("contact-1", "Contact One");

            _context.Users.AddRange(currentUser, contact);
            await _context.SaveChangesAsync();

            // Criar uma conversa onde o ContactId é o usuário atual e o UserId é o contato
            var existingConversation = new Conversation
            {
                UserId = contact.Id,
                User = contact,
                ContactId = _testUserId,
                Contact = currentUser,
                Messages = new List<Message>()
            };

            _context.Conversations.Add(existingConversation);
            await _context.SaveChangesAsync();

            // Adicionar a mensagem à conversa com o ConversationId
            var existingMessage = new Message
            {
                SenderId = contact.Id,
                Sender = contact,
                ReceiverId = _testUserId,
                Receiver = currentUser,
                Content = "Message from contact",
                Timestamp = DateTime.UtcNow.AddMinutes(-10),
                ConversationId = existingConversation.Id,
                Conversation = existingConversation
            };

            _context.Messages.Add(existingMessage);
            await _context.SaveChangesAsync();

            var messageDto = new ChatMessageDto
            {
                ReceiverId = contact.Id,
                Content = "Response to contact"
            };

            // Agir
            var result = await _controller.SaveMessage(messageDto);

            // Verificar
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var savedMessage = Assert.IsType<MessageDto>(okResult.Value);

            // Verificar se a mensagem foi adicionada à conversa existente
            var updatedConversation = await _context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == existingConversation.Id);

            Assert.NotNull(updatedConversation);
            Assert.Equal(2, updatedConversation.Messages.Count);
            Assert.Contains(updatedConversation.Messages, m => m.Content == messageDto.Content);
        }

        [Fact]
        public async Task GetConversations_ReturnsUnauthorized_WhenUserIdNotFound()
        {
            // Arranjar - Substituir o HttpContext por um sem identificação de usuário
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            // Agir
            var result = await _controller.GetConversations();

            // Verificar
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task GetAvailableContacts_ReturnsUnauthorized_WhenUserIdNotFound()
        {
            // Arranjar - Substituir o HttpContext por um sem identificação de usuário
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            // Agir
            var result = await _controller.GetAvailableContacts();

            // Verificar
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task SaveMessage_ReturnsUnauthorized_WhenUserIdNotFound()
        {
            // Arranjar - Substituir o HttpContext por um sem identificação de usuário
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var messageDto = new ChatMessageDto
            {
                ReceiverId = "contact-1",
                Content = "Test message"
            };

            // Agir
            var result = await _controller.SaveMessage(messageDto);

            // Verificar
            Assert.IsType<UnauthorizedResult>(result.Result);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace SoliGest.Server.Controllers
{
    // DTO definitions
    public class ContactDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ConversationDto
    {
        public int Id { get; set; }
        public ContactDto Contact { get; set; }
        public List<MessageDto> Messages { get; set; }
    }

    public class ChatMessageDto
    {
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly SoliGestServerContext _context;

        public ChatController(SoliGestServerContext context)
        {
            _context = context;
        }

        // GET: api/Chat/conversations
        [HttpGet("conversations")]
        public async Task<ActionResult<IEnumerable<ConversationDto>>> GetConversations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var convs = await _context.Conversations
                .Where(c => c.UserId == userId)
                .Select(c => new ConversationDto
                {
                    Id = c.Id,
                    Contact = new ContactDto
                    {
                        Id = c.Contact.Id,
                        Name = c.Contact.Name
                    },
                    Messages = c.Messages
                        .Select(m => new MessageDto
                        {
                            Id = m.Id,
                            SenderId = m.SenderId,
                            ReceiverId = m.ReceiverId,
                            Content = m.Content,
                            Timestamp = m.Timestamp
                        })
                        .OrderBy(m => m.Timestamp)
                        .ToList()
                })
                .ToListAsync();

            return Ok(convs);
        }

        // GET: api/Chat/contacts
        [HttpGet("contacts")]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetAvailableContacts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var contacts = await _context.Users
                .Where(u => u.Id != userId)
                .Select(u => new ContactDto
                {
                    Id = u.Id,
                    Name = u.Name
                })
                .ToListAsync();

            return Ok(contacts);
        }

        // POST: api/Chat/message
        [HttpPost("message")]
        public async Task<ActionResult<MessageDto>> SaveMessage([FromBody] ChatMessageDto messageDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Load user and contact entities for navigation properties
            var userEntity = await _context.Users.FindAsync(userId)
                ?? throw new InvalidOperationException("User not found");
            var contactEntity = await _context.Users.FindAsync(messageDto.ReceiverId)
                ?? throw new InvalidOperationException("Contact not found");

            // Find existing conversation or create new
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c =>
                    (c.UserId == userId && c.ContactId == messageDto.ReceiverId) ||
                    (c.UserId == messageDto.ReceiverId && c.ContactId == userId));

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    UserId = userId,
                    User = userEntity,
                    ContactId = messageDto.ReceiverId,
                    Contact = contactEntity,
                    Messages = new List<Message>()
                };
                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }

            // Create message entity with required navigation
            var message = new Message
            {
                Content = messageDto.Content,
                Timestamp = DateTime.UtcNow,
                SenderId = userId,
                Sender = userEntity,
                ReceiverId = messageDto.ReceiverId,
                Receiver = contactEntity,
                ConversationId = conversation.Id,
                Conversation = conversation
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var result = new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                Timestamp = message.Timestamp
            };

            return Ok(result);
        }
    }
}

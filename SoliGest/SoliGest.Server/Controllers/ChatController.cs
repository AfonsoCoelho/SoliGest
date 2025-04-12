using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models; // Ajusta este namespace conforme a estrutura dos teus modelos
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SoliGest.Server.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly SoliGestServerContext _context;
        public ChatController(SoliGestServerContext context)
        {
            _context = context;
        }

        // GET: api/Chat/conversations
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            // Obtem o ID do utilizador a partir do JWT
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Obtem as conversas em que o user participou
            var conversations = await _context.Conversations
                .Include(c => c.Messages)
                .Include(c => c.Contact)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return Ok(conversations);
        }

        // GET: api/Chat/contacts
        [HttpGet("contacts")]
        public async Task<IActionResult> GetAvailableContacts()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Exemplo: Buscar todos os utilizadores que não sejam o utilizador atual.
            var contacts = await _context.Users
                .Where(u => u.Id != userId)
                .Select(u => new
                {
                    u.Id,
                    u.Name
                })
                .ToListAsync();

            return Ok(contacts);
        }

        // POST: api/Chat/message
        [HttpPost("message")]
        public async Task<IActionResult> SaveMessage([FromBody] ChatMessageDto messageDto)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => (c.UserId == userId && c.ContactId == messageDto.ReceiverId) ||
                                          (c.UserId == messageDto.ReceiverId && c.ContactId == userId));

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    UserId = userId,
                    User = await _context.Users.FindAsync(userId) ?? throw new InvalidOperationException("User not found."),
                    ContactId = messageDto.ReceiverId,
                    Contact = await _context.Users.FindAsync(messageDto.ReceiverId) ?? throw new InvalidOperationException("Contact not found.")
                };
                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }

            var message = new Message
            {
                SenderId = userId,
                Sender = await _context.Users.FindAsync(userId),
                ReceiverId = messageDto.ReceiverId,
                Receiver = await _context.Users.FindAsync(messageDto.ReceiverId),
                Content = messageDto.Content,
                Timestamp = DateTime.UtcNow,
                ConversationId = conversation.Id,
                Conversation = conversation
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(message);
        }
    }

    // DTO para receber a mensagem na requisição
    public class ChatMessageDto
    {
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }
}

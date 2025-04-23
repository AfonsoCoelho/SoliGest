using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SoliGest.Server.Hubs;
using SoliGest.Server.Models;        // ajustar namespace
using SoliGest.Server.Repositories;  // o teu repositório de chat
using System;
using System.Threading.Tasks;

namespace SoliGest.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatRepository _repo;

        public ChatController(IHubContext<ChatHub> hubContext, IChatRepository repo)
        {
            _hubContext = hubContext;
            _repo = repo;
        }

        // GET: api/chat/conversations
        [HttpGet("conversations:{userId}")]
        public async Task<IActionResult> GetConversationsFor(string userId)
        {
            if (userId == null) return Unauthorized();

            var convos = await _repo.GetConversationsFor(userId);
            return Ok(convos);
        }

        // POST: api/chat/message
        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageDto dto)
        {
            var senderId = User.FindFirst("sub")?.Value;
            if (senderId == null) return Unauthorized();

            var timestamp = DateTime.UtcNow;

            await _repo.SaveMessage(senderId, dto.ReceiverId, dto.Content, timestamp);

            await _hubContext.Clients.User(dto.ReceiverId)
                .SendAsync("ReceiveMessage", senderId, dto.Content, timestamp);

            return Ok();
        }

        [HttpGet("contacts")]
        public async Task<IActionResult> GetAvailableContacts()
        {
            var userId = User.FindFirst("sub")?.Value;
            if (userId == null) return Unauthorized();

            var contacts = await _repo.GetAvailableContacts(userId);
            return Ok(contacts);
        }

    }

    // DTO usado no POST
    public class ChatMessageDto
    {
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
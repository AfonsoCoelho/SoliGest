using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoliGest.Server.Repositories;
using System.Security.Claims;

namespace SoliGest.Server.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _repo;
        public ChatController(IChatRepository repo) => _repo = repo;

        [HttpGet("history/{otherUserId}")]
        public async Task<IActionResult> GetHistory(string otherUserId)
        {
            var me = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var history = await _repo.GetConversationAsync(me, otherUserId);
            return Ok(history);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SoliGest.Server.Hubs;
using SoliGest.Server.Models;
using SoliGest.Server.Repositories;
using System;
using System.Threading.Tasks;

namespace SoliGest.Server.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão do sistema de chat entre utilizadores.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatRepository _repo;

        /// <summary>
        /// Construtor do controlador de chat.
        /// </summary>
        /// <param name="hubContext">Contexto do Hub SignalR para envio de mensagens em tempo real.</param>
        /// <param name="repo">Repositório responsável por persistir mensagens e conversas.</param>
        public ChatController(IHubContext<ChatHub> hubContext, IChatRepository repo)
        {
            _hubContext = hubContext;
            _repo = repo;
        }

        /// <summary>
        /// Obtém todas as conversas associadas ao utilizador autenticado.
        /// </summary>
        /// <returns>Lista de conversas do utilizador.</returns>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = User.FindFirst("sub")?.Value;
            var convos = await _repo.GetConversationsFor(userId);
            return Ok(convos);
        }

        /// <summary>
        /// Envia uma nova mensagem para um utilizador.
        /// </summary>
        /// <param name="dto">Objeto com os dados da mensagem, incluindo destinatário e conteúdo.</param>
        /// <returns>Resposta HTTP indicando sucesso da operação.</returns>
        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageDto dto)
        {
            var senderId = User.FindFirst("sub")?.Value;
            var timestamp = DateTime.UtcNow;

            await _repo.SaveMessage(senderId, dto.ReceiverId, dto.Content, timestamp);

            await _hubContext.Clients.User(dto.ReceiverId)
                .SendAsync("ReceiveMessage", senderId, dto.Content, timestamp);

            return Ok();
        }
    }

    /// <summary>
    /// Data Transfer Object (DTO) utilizado para envio de mensagens no chat.
    /// </summary>
    public class ChatMessageDto
    {
        /// <summary>
        /// ID do utilizador que irá receber a mensagem.
        /// </summary>
        public string ReceiverId { get; set; }

        /// <summary>
        /// Conteúdo textual da mensagem.
        /// </summary>
        public string Content { get; set; }
    }
}

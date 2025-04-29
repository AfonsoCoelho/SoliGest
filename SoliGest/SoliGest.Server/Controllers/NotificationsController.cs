using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using System.Data;

namespace SoliGest.Server.Controllers
{
    /// <summary>
    /// Controlador para gerenciar as notificações.
    /// Permite operações CRUD para as notificações.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : Controller
    {
        private readonly SoliGestServerContext _context;

        /// <summary>
        /// Construtor do controlador NotificationsController.
        /// Inicializa o contexto do banco de dados para realizar operações sobre as notificações.
        /// </summary>
        /// <param name="context">O contexto do banco de dados.</param>
        public NotificationsController(SoliGestServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todas as notificações.
        /// </summary>
        /// <returns>Uma lista de todas as notificações.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
        {
            return await _context.Notification.ToListAsync();
        }

        /// <summary>
        /// Cria uma nova notificação.
        /// </summary>
        /// <param name="notification">A notificação a ser criada.</param>
        /// <returns>Um status de resposta que inclui a notificação criada.</returns>
        /// <response code="201">Retorna a notificação criada.</response>
        /// <response code="400">Em caso de erro ao salvar a notificação.</response>
        [HttpPost]
        public async Task<IActionResult> PostNotification(Notification notification)
        {
            try
            {
                _context.Notification.Add(notification);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetNotification", new { id = notification.Id }, notification);
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Deleta uma notificação existente.
        /// </summary>
        /// <param name="id">O ID da notificação a ser deletada.</param>
        /// <returns>Status de resposta com o resultado da exclusão.</returns>
        /// <response code="200">Notificação deletada com sucesso.</response>
        /// <response code="404">Notificação não encontrada.</response>
        /// <response code="400">Erro ao tentar deletar a notificação.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
            {
                var notification = await _context.FindAsync<Notification>(id);
                if (notification == null)
                {
                    return NotFound($"Não foi possível encontrar a notificação com o ID '{id}'.");
                }

                _context.Remove(notification);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Obtém uma notificação específica.
        /// </summary>
        /// <param name="id">O ID da notificação a ser retornada.</param>
        /// <returns>A notificação solicitada.</returns>
        /// <response code="200">Retorna a notificação encontrada.</response>
        /// <response code="404">Notificação não encontrada.</response>
        /// <response code="400">Erro ao tentar buscar a notificação.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            try
            {
                var notification = await _context.FindAsync<Notification>(id);
                if (notification != null)
                {
                    return notification;
                }
                else
                {
                    return NotFound($"Não foi possível encontrar a notificação com o ID '{id}'.");
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Atualiza uma notificação existente.
        /// </summary>
        /// <param name="updatedNotification">A notificação com os dados atualizados.</param>
        /// <returns>Status de resposta com o resultado da atualização.</returns>
        /// <response code="200">Notificação atualizada com sucesso.</response>
        /// <response code="404">Notificação não encontrada.</response>
        /// <response code="400">Erro ao tentar atualizar a notificação.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotification(Notification updatedNotification)
        {
            var notification = await _context.FindAsync<Notification>(updatedNotification.Id);
            if (notification == null)
            {
                return NotFound($"Não foi possível encontrar a notificação com o ID '{updatedNotification.Id}'.");
            }

            notification.Title = updatedNotification.Title;
            notification.Type = updatedNotification.Type;
            notification.Message = updatedNotification.Message;

            _context.Update(notification);

            var result = _context.SaveChanges();

            if (result != 1)
            {
                return BadRequest("Ocorreu um erro!");
            }

            return Ok(new { message = "Notificação atualizada com sucesso!" });
        }
    }
}

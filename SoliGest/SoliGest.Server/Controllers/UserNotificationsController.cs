using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using SoliGest.Server.Services;

namespace SoliGest.Server.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão das notificações dos utilizadores.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserNotificationsController : Controller
    {
        private readonly SoliGestServerContext _context;
        private readonly IUserNotificationService _userNotificationService;

        /// <summary>
        /// Construtor do controlador de notificações dos utilizadores.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userNotificationService">Serviço para notificação de utilizadores.</param>
        public UserNotificationsController(SoliGestServerContext context, IUserNotificationService userNotificationService)
        {
            _context = context;
            _userNotificationService = userNotificationService;
        }

        /// <summary>
        /// Obtém todas as notificações de utilizadores.
        /// </summary>
        /// <returns>Lista de notificações com os utilizadores e notificações associadas.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserNotification>>> GetUserNotifications()
        {
            return await _context.UserNotification.Include(uN => uN.User).Include(uN => uN.Notification).ToListAsync();
        }

        /// <summary>
        /// Cria uma nova associação de notificação a um utilizador.
        /// </summary>
        /// <param name="model">Modelo de dados para a notificação do utilizador.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPost]
        public async Task<IActionResult> PostUserNotification([FromBody] UserNotificationUpdateModel model)
        {
            try
            {
                await _userNotificationService.PostUserNotification(model);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Elimina uma notificação de utilizador com base no seu ID.
        /// </summary>
        /// <param name="id">ID da notificação do utilizador.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserNotification(int id)
        {
            try
            {
                var userNotification = await _context.FindAsync<UserNotification>(id);
                if (userNotification == null)
                {
                    return NotFound($"Não foi possível encontrar a notificação com o ID '{id}'.");
                }

                _context.Remove(userNotification);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Obtém uma notificação de utilizador por ID.
        /// </summary>
        /// <param name="id">ID da notificação de utilizador.</param>
        /// <returns>Notificação do utilizador correspondente.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserNotification>> GetUserNotification(int id)
        {
            try
            {
                var userNotification = await _context.UserNotification
                    .Include(uN => uN.User)
                    .Include(uN => uN.Notification)
                    .FirstOrDefaultAsync(uN => uN.UserNotificationId == id);

                if (userNotification != null)
                {
                    return userNotification;
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
        /// Atualiza os dados de uma notificação de utilizador existente.
        /// </summary>
        /// <param name="model">Modelo com os novos dados da notificação.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserNotification([FromBody] UserNotificationUpdateModel model)
        {
            var userNotification = await _context.FindAsync<UserNotification>(model.UserNotificationId);
            if (userNotification == null)
            {
                return NotFound($"Não foi possível encontrar a notificação com o ID '{model.UserNotificationId}'.");
            }

            userNotification.UserId = model.UserId;

            var user = await _context.FindAsync<User>(userNotification.UserId);
            if (user != null)
            {
                userNotification.User = user;
            }
            else
            {
                return BadRequest("O id de utilizador indicado não está associado a nenhum utilizador.");
            }

            userNotification.NotificationId = model.NotificationId;

            var notification = await _context.FindAsync<Notification>(userNotification.NotificationId);
            if (notification != null)
            {
                userNotification.Notification = notification;
            }
            else
            {
                return BadRequest("O id de notificação indicado não está associado a nenhuma notificação.");
            }

            userNotification.ReceivedDate = model.ReceivedDate;
            userNotification.IsRead = model.IsRead;

            _context.Update(userNotification);

            var result = _context.SaveChanges();

            if (result != 1)
            {
                return BadRequest("Ocorreu um erro!");
            }

            return Ok(new { message = "Notificação atualizada com sucesso!" });
        }

        /// <summary>
        /// Marca uma notificação de utilizador como lida.
        /// </summary>
        /// <param name="userNotificationId">ID da notificação do utilizador.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPut("{id}/markAsRead")]
        public async Task<IActionResult> MarkNotificationAsRead(int userNotificationId)
        {
            var notification = await _context.FindAsync<UserNotification>(userNotificationId);
            if (notification == null)
            {
                return NotFound($"Não foi possível encontrar a notificação com o ID '{userNotificationId}'.");
            }

            notification.IsRead = true;
            _context.Update(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notificação marcada como lida com sucesso." });
        }

        /// <summary>
        /// Obtém todas as notificações de um utilizador específico.
        /// </summary>
        /// <param name="userId">ID do utilizador.</param>
        /// <returns>Lista de notificações do utilizador.</returns>
        [HttpGet("ByUserId/{userId}")]
        public async Task<IActionResult> GetUserNotificationsByUserId(string userId)
        {
            var userNotifications = await _context.UserNotification
                .Where(un => un.UserId.Equals(userId))
                .Include(un => un.User)
                .Include(un => un.Notification)
                .ToListAsync();

            if (userNotifications == null || userNotifications.Count == 0)
            {
                return NotFound($"Não foram encontradas notificações para o utilizador com ID '{userId}'.");
            }

            return Ok(userNotifications);
        }

        /// <summary>
        /// Modelo de atualização de notificações de utilizador.
        /// </summary>
        public class UserNotificationUpdateModel
        {
            /// <summary>
            /// ID da notificação do utilizador.
            /// </summary>
            public int UserNotificationId { get; set; }

            /// <summary>
            /// ID do utilizador.
            /// </summary>
            public required string UserId { get; set; }

            /// <summary>
            /// ID da notificação.
            /// </summary>
            public int NotificationId { get; set; }

            /// <summary>
            /// Data em que a notificação foi recebida.
            /// </summary>
            public DateTime ReceivedDate { get; set; }

            /// <summary>
            /// Define se a notificação foi lida.
            /// </summary>
            public bool IsRead { get; set; }
        }
    }
}

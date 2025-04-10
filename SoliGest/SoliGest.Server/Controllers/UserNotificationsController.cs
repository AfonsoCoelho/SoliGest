using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

namespace SoliGest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserNotificationsController : Controller
    {
        private readonly SoliGestServerContext _context;

        public UserNotificationsController(SoliGestServerContext context)
        {
            _context = context;
        }

        // GET: api/UserNotifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserNotification>>> GetUserNotifications()
        {
            return await _context.UserNotification.ToListAsync();
        }

        // POST: api/UserNotifications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostUserNotification(UserNotification userNotification)
        {
            try
            {
                _context.UserNotification.Add(userNotification);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUserNotification", new { id = userNotification.UserNotificationId }, userNotification);
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE: api/UserNotifications/5
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

        // GET: api/UserNotifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserNotification>> GetUserNotification(int id)
        {
            try
            {
                var userNotification = await _context.FindAsync<UserNotification>(id);
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

        // PUT: api/UserNotifications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserNotification([FromBody]UserNotificationUpdateModel model)
        {
            var userNotification = await _context.FindAsync<UserNotification>();
            if (userNotification == null)
            {
                return NotFound($"Não foi possível encontrar a notificação com o ID '{model.UserNotificationId}'.");
            }

            userNotification.UserId = model.UserId;
            var user = _context.FindAsync<User>(userNotification.UserId);
            if(user.Result != null)
            {
                userNotification.User = user.Result;
            }
            else
            {
                return BadRequest("O id de uilizador indicado não está associado a nenhum utilizador.");
            }
            userNotification.NotificationId = model.NotificationId;
            var notification = _context.FindAsync<Notification>(userNotification.NotificationId);
            if (notification.Result != null)
            {
                userNotification.Notification = notification.Result;
            }
            else
            {
                return BadRequest("O id de notificação indicado não está associado a nenhum utilizador.");
            }
            userNotification.ReceivedDate = model.ReceivedDate;
            userNotification.isRead = model.isRead;

            _context.Update(userNotification);

            var result = _context.SaveChanges();

            if (result != 1)
            {
                return BadRequest("Ocorreu um erro!");
            }

            return Ok(new { message = "Notificação atualizada com sucesso!" });
        }

        public class UserNotificationUpdateModel
        {
            public int UserNotificationId { get; set; }
            public string UserId { get; set; }
            public int NotificationId { get; set; }
            public DateTime ReceivedDate { get; set; }
            public bool isRead { get; set; }
        }
    }
}

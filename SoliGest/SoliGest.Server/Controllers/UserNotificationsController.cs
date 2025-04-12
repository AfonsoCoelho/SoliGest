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
            return await _context.UserNotification.Include(uN => uN.User).Include(uN => uN.Notification).ToListAsync();
        }

        // POST: api/UserNotifications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostUserNotification([FromBody]UserNotificationUpdateModel model)
        {
            try
            {
                User user = await _context.FindAsync<User>(model.UserId);
                if(user == null)
                {
                    return BadRequest("O utilizador com o Id fornecido não foi encontrado.");
                }
                Notification notification = await _context.FindAsync<Notification>(model.NotificationId);
                if (notification == null)
                {
                    return BadRequest("A notificação com o Id fornecido não foi encontrada.");
                }
                UserNotification userNotification = new UserNotification
                {
                    UserNotificationId = 0,
                    User = user,
                    UserId = model.UserId,
                    Notification = notification,
                    NotificationId = model.NotificationId,
                    ReceivedDate = DateTime.Now,
                    isRead = false
                };
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
                var userNotification = await _context.UserNotification.Include(uN => uN.User).Include(uN => uN.Notification).FirstOrDefaultAsync(uN => uN.UserNotificationId == id);
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
            var userNotification = await _context.FindAsync<UserNotification>(model.UserNotificationId);
            if (userNotification == null)
            {
                return NotFound($"Não foi possível encontrar a notificação com o ID '{model.UserNotificationId}'.");
            }

            userNotification.UserId = model.UserId;
            var user = await _context.FindAsync<User>(userNotification.UserId);
            if(user != null)
            {
                userNotification.User = user;
            }
            else
            {
                return BadRequest("O id de uilizador indicado não está associado a nenhum utilizador.");
            }
            userNotification.NotificationId = model.NotificationId;
            var notification = await _context.FindAsync<Notification>(userNotification.NotificationId);
            if (notification != null)
            {
                userNotification.Notification = notification;
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

        [HttpPut("{id}/markAsRead")]
        public async Task<IActionResult> MarkNotificationAsRead(int userNotificationId)
        {
            var notification = await _context.FindAsync<UserNotification>(userNotificationId);
            if(notification == null)
            {
                return NotFound($"Não foi possível encontrar a notificação com o ID '{RouteData.Values}'.");
            }
            return Ok();
        }

        [HttpGet("/ByUserId/{userId}")]
        public async Task<IActionResult> GetUserNotificationsByUserId(string userId)
        {
            var userNotifications = await _context.UserNotification.Where(un => un.UserId.Equals(userId)).Include(un => un.User).Include(un => un.Notification).ToListAsync();
            Console.WriteLine(RouteData);
            if (userNotifications == null)
            {
                return NotFound($"Não foi possível encontrar a notificação com o ID '{RouteData}'.");
            }
            return Ok(userNotifications);
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

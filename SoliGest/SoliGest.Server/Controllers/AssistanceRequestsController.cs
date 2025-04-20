using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using SoliGest.Server.Services;
using static SoliGest.Server.Controllers.UserNotificationsController;

namespace SoliGest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssistanceRequestsController : Controller
    {
        private readonly SoliGestServerContext _context;
        private readonly IUserNotificationService _userNotificationService;

        public AssistanceRequestsController(SoliGestServerContext context, IUserNotificationService userNotificationService)
        {
            _context = context;
            _userNotificationService = userNotificationService;
        }

        // GET: api/AssistanceRequest
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssistanceRequest>>> GetAll()
        {
            return await _context.AssistanceRequest.Include(a => a.SolarPanel).Include(a => a.AssignedUser).ToListAsync();
        }

        // GET: api/AssistanceRequest/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssistanceRequest>> GetById(int id)
        {
            var request = await _context.AssistanceRequest.Include(a => a.SolarPanel).Include(a => a.AssignedUser).FirstOrDefaultAsync(a => a.Id == id);

            if (request == null)
            {
                return NotFound(new { message = "Pedido de assistência não encontrado." });
            }

            return Ok(request);
        }

        // POST: api/AssistanceRequest
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]AssistanceRequestCreateModel request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Dados inválidos." });
            }

            var solarPanel = await _context.FindAsync<SolarPanel>(request.SolarPanelId);

            if(solarPanel == null)
            {
                return BadRequest(new { message = "Painel solar não encontrado." });
            }

            var assistanceRequest = new AssistanceRequest
            {
                Id = 0,
                RequestDate = request.RequestDate,
                Priority = request.Priority,
                Status = request.Status,
                StatusClass = request.StatusClass,
                ResolutionDate = request.ResolutionDate,
                Description = request.Description,
                SolarPanel = solarPanel
            };

            if (!string.IsNullOrEmpty(request.AssignedUserId))
            {
                var user = await _context.Users.FindAsync(request.AssignedUserId);
                if (user == null)
                    return BadRequest(new { message = "Utilizador atribuído não encontrado." });

                assistanceRequest.AssignedUser = user;
            }


            await _context.AssistanceRequest.AddAsync(assistanceRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = assistanceRequest.Id }, request);
        }

        // PUT: api/AssistanceRequest/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AssistanceRequestUpdateModel request)
        {
            //if (id != request.Id)
            //{
            //    return BadRequest(new { message = "ID do pedido não corresponde." });
            //}

            var solarPanel = await _context.SolarPanel.FindAsync(request.SolarPanelId);

            if(solarPanel == null)
            {
                return BadRequest(new { message = "Painel Solar inexistente!" });
            }

            var assistanceRequest = await _context.FindAsync<AssistanceRequest>(id);

            if(assistanceRequest == null)
            {
                return NotFound($"Não foi possível encontrar a assistência técnica com o ID '{id}'.");
            }

            assistanceRequest.Priority = request.Priority;
            assistanceRequest.Status = request.Status;
            assistanceRequest.StatusClass = request.StatusClass;
            assistanceRequest.SolarPanel = solarPanel;
            assistanceRequest.Description = request.Description;
            assistanceRequest.RequestDate = request.RequestDate;
            assistanceRequest.ResolutionDate = request.ResolutionDate;

            if (!string.IsNullOrEmpty(request.AssignedUserId))
            {
                var user = await _context.Users.FindAsync(request.AssignedUserId);
                if (user == null)
                    return BadRequest(new { message = "Utilizador atribuído não encontrado." });

                if (assistanceRequest.AssignedUser != user)
                {
                    UserNotificationUpdateModel unum = new UserNotificationUpdateModel {
                        UserNotificationId = 0,
                        UserId = user.Id,
                        NotificationId = 1,
                        IsRead = false,
                        ReceivedDate = DateTime.Now
                    };

                    await _userNotificationService.PostUserNotification(unum);
                }

                assistanceRequest.AssignedUser = user;                
            }
            //else
            //{
            //    assistanceRequest.AssignedUser = null;
            //}

            _context.Update(assistanceRequest);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return NotFound(new { message = "Pedido de assistência não encontrado." });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Pedido de assistência atualizado com sucesso." });
        }

        // DELETE: api/AssistanceRequest/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.AssistanceRequest.FindAsync(id);
            if (request == null)
            {
                return NotFound(new { message = "Pedido de assistência não encontrado." });
            }

            _context.AssistanceRequest.Remove(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pedido de assistência removido com sucesso." });
        }

        private bool Exists(int id)
        {
            return _context.AssistanceRequest.Any(e => e.Id == id);
        }

    }

    public class AssistanceRequestUpdateModel
    {
        public string RequestDate { get; set; }
        public string ResolutionDate { get; set; }
        public string Description { get; set; }
        public int SolarPanelId { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string StatusClass { get; set; }
        public string? AssignedUserId { get; set; }
    }
    public class AssistanceRequestCreateModel
    {
        public string RequestDate { get; set; }
        public string ResolutionDate { get; set; }
        public string Description { get; set; }
        public int SolarPanelId { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string StatusClass { get; set; }
        public string? AssignedUserId { get; set; }
    }
}

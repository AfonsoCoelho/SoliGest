using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

namespace SoliGest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssistanceRequestsController : Controller
    {
        private readonly SoliGestServerContext _context;

        public AssistanceRequestsController(SoliGestServerContext context)
        {
            _context = context;
        }

        // GET: api/AssistanceRequest
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssistanceRequest>>> GetAll()
        {
            return await _context.AssistanceRequest.Include(a => a.SolarPanel).ToListAsync();
        }

        // GET: api/AssistanceRequest/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssistanceRequest>> GetById(int id)
        {
            var request = await _context.AssistanceRequest.Include(a => a.SolarPanel).FirstOrDefaultAsync(a => a.Id == id);

            if (request == null)
            {
                return NotFound(new { message = "Pedido de assistência não encontrado." });
            }

            return Ok(request);
        }

        // POST: api/AssistanceRequest
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AssistanceRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Dados inválidos." });
            }

            _context.AssistanceRequest.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
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

            assistanceRequest.SolarPanel = solarPanel;
            assistanceRequest.Description = request.Description;
            assistanceRequest.RequestDate = request.RequestDate;
            assistanceRequest.ResolutionDate = request.ResolutionDate;

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
    }
}

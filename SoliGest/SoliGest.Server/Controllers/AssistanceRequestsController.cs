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
            return await _context.AssistanceRequest.ToListAsync();
        }

        // GET: api/AssistanceRequest/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssistanceRequest>> GetById(int id)
        {
            var request = await _context.AssistanceRequest.FindAsync(id);

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
        public async Task<IActionResult> Update(int id, [FromBody] AssistanceRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest(new { message = "ID do pedido não corresponde." });
            }

            _context.Entry(request).State = EntityState.Modified;

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
}

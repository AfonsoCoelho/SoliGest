using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

namespace SoliGest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolarPanelsController : Controller
    {
        private readonly SoliGestServerContext _context;

        public SolarPanelsController(SoliGestServerContext context)
        {
            _context = context;
        }

        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolarPanel>>> GetSolarPanel()
        {
            return await _context.SolarPanel.ToListAsync();
        }

        // POST: api/People
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostSolarPanel(SolarPanel solarPanel)
        {
            try
            {
                _context.SolarPanel.Add(solarPanel);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSolarPanel", new { id = solarPanel.Id }, solarPanel);
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE: api/People/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolarPanel(int id)
        {
            try
            {
                var solarPanel = await _context.FindAsync<SolarPanel>(id);
                if(solarPanel == null)
                {
                    return NotFound($"Não foi possível encontrar o painel solar com o ID '{id}'.");
                }

                _context.Remove<SolarPanel>(solarPanel);
                await _context.SaveChangesAsync();

                return Ok("Painel solar removido com sucesso!");
            }
            catch
            {
                return BadRequest();
            }
        }

        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SolarPanel>> GetSolarPanel(int id)
        {
            try
            {
                var solarPanel = await _context.FindAsync<SolarPanel>(id);
                if(solarPanel != null)
                {
                    return solarPanel;
                }
                else
                {
                    return NotFound($"Não foi possível encontrar o painel solar com o ID '{id}'.");
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSolarPanel([FromBody] SolarPanelUpdateModel model)
        {
            var solarPanel = await _context.FindAsync<SolarPanel>(model.Id);
            if (solarPanel == null)
            {
                return NotFound($"Não foi possível encontrar o painel solar com o ID '{model.Id}'.");
            }

            solarPanel.PhoneNumber = model.PhoneNumber;
            solarPanel.Email = model.Email;
            solarPanel.Address = model.Address;

            _context.Update<SolarPanel>(solarPanel);

            var result = _context.SaveChanges();

            if (result != 1)
            {
                return BadRequest("Ocorreu um erro!");
            }

            return Ok(new { message = "Painel solar atualizado com sucesso!" });
        }
    }

    public class SolarPanelUpdateModel
    {
        public int Id { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using SoliGest.Server.Services;

namespace SoliGest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolarPanelsController : Controller
    {
        private readonly SoliGestServerContext _context;
        private readonly IGeoCodingService _geo;

        
        public SolarPanelsController(SoliGestServerContext context, IGeoCodingService geo)
        {
            _context = context;
            _geo = geo;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolarPanel>>> GetSolarPanel()
        {
            return await _context.SolarPanel.ToListAsync();
        }

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostSolarPanel(SolarPanel solarPanel)
        {
            SolarPanel result = this.GetGeocodeResult(solarPanel);

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

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

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

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSolarPanel([FromBody] SolarPanelUpdateModel model)
        {
            var solarPanel = await _context.FindAsync<SolarPanel>(model.Id);
            if (solarPanel == null)
            {
                return NotFound($"Não foi possível encontrar o painel solar com o ID '{model.Id}'.");
            }

            solarPanel.Name = model.Name;
            solarPanel.Priority = model.Priority;
            solarPanel.Status = model.Status;
            solarPanel.StatusClass = model.StatusClass;
            solarPanel.Description = model.Description;
            solarPanel.PhoneNumber = model.PhoneNumber;
            solarPanel.Email = model.Email;
            solarPanel.Address = model.Address;

            solarPanel = this.GetGeocodeResult(solarPanel);

            _context.Update<SolarPanel>(solarPanel);

            var result = _context.SaveChanges();

            if (result != 1)
            {
                return BadRequest("Ocorreu um erro!");
            }

            return Ok(new { message = "Painel solar atualizado com sucesso!" });
        }
        
        private SolarPanel GetGeocodeResult(SolarPanel solarPanel)
        {
            GeocodeResult? result =  _geo.GeocodeAsync(solarPanel.Address).Result;
            solarPanel.Latitude = result?.Latitude ?? 0;
            solarPanel.Longitude = result?.Longitude ?? 0;

            return solarPanel;
        }
    }

    public class SolarPanelUpdateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string StatusClass { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}

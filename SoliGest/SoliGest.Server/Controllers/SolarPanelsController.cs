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
    public class SolarPanelsController : Controller
    {
        private readonly SoliGestServerContext _context;

        public SolarPanelsController(SoliGestServerContext context)
        {
            _context = context;
        }

        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolarPanel>>> GetPerson()
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
    }
}

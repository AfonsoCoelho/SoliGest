using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

namespace SoliGest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DaysOffController : Controller
    {
        private readonly SoliGestServerContext _context;

        public DaysOffController(SoliGestServerContext context)
        {
            _context = context;
        }

        // Recebe um array de datas e salva cada uma como Folga
        [HttpPost("{userId}")]
        public async Task<IActionResult> SaveFolgas(string userId, [FromBody] List<DateTime> datas)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("Usuário não encontrado.");

            // Este n sei se faz sentido deixar assim
            // var oldFolgas = _context.Folgas.Where(f => f.UserId == userId);
            // _context.Folgas.RemoveRange(oldFolgas);

            var novasFolgas = datas.Select(d => new DayOff
            {
                UserId = userId,
                Day = DateOnly.FromDateTime(d)
            });

            await _context.DaysOff.AddRangeAsync(novasFolgas);
            await _context.SaveChangesAsync();

            return Ok(novasFolgas);
        }
    }
}

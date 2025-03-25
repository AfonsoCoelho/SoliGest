using Microsoft.AspNetCore.Mvc;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

namespace SoliGest.Server.Controllers
{
    public class HolidaysController : Controller
    {
        private readonly SoliGestServerContext _context;

        public HolidaysController(SoliGestServerContext context)
        {
            _context = context;
        }

        public class HolidaysDto
        {
            public DateTime inicio { get; set; }
            public DateTime fim { get; set; }
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> SaveHolidays(string userId, [FromBody] List<HolidaysDto> holidays
            )
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("Usuário não encontrado.");

            var oldHolidays = _context.Holidays.Where(f => f.UserId == userId);
            if (oldHolidays != null) _context.Holidays.RemoveRange(oldHolidays);

            var novasFerias = holidays.Select(f => new Holidays
            {
                UserId = userId,
                HolidayStart = DateOnly.FromDateTime(f.inicio),
                HolidayEnd = DateOnly.FromDateTime(f.fim)
            });

            await _context.Holidays.AddRangeAsync(novasFerias);
            await _context.SaveChangesAsync();

            return Ok(novasFerias);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

namespace SoliGest.Server.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de férias dos utilizadores.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HolidaysController : Controller
    {
        private readonly SoliGestServerContext _context;

        /// <summary>
        /// Construtor do controlador de férias.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        public HolidaysController(SoliGestServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Objeto DTO para receber períodos de férias.
        /// </summary>
        public class HolidaysDto
        {
            /// <summary>
            /// Data de início das férias.
            /// </summary>
            public DateTime inicio { get; set; }

            /// <summary>
            /// Data de fim das férias.
            /// </summary>
            public DateTime fim { get; set; }
        }

        /// <summary>
        /// Regista os períodos de férias de um utilizador, substituindo os anteriores.
        /// </summary>
        /// <param name="userId">Identificador do utilizador.</param>
        /// <param name="holidays">Lista de períodos de férias a serem registados.</param>
        /// <returns>Lista de férias registadas ou mensagem de erro.</returns>
        [HttpPost("{userId}")]
        public async Task<IActionResult> SaveHolidays(string userId, [FromBody] List<HolidaysDto> holidays)
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

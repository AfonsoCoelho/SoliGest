using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

namespace SoliGest.Server.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de dias de folga dos utilizadores.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DaysOffController : Controller
    {
        private readonly SoliGestServerContext _context;

        /// <summary>
        /// Construtor do controlador de folgas.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        public DaysOffController(SoliGestServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Regista um conjunto de datas como folgas para um determinado utilizador.
        /// </summary>
        /// <param name="userId">Identificador do utilizador a quem as folgas pertencem.</param>
        /// <param name="datas">Lista de datas a serem registadas como folgas.</param>
        /// <returns>Lista de folgas criadas ou erro apropriado.</returns>
        [HttpPost("{userId}")]
        public async Task<IActionResult> SaveFolgas(string userId, [FromBody] List<DateTime> datas)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("Usuário não encontrado.");

            // NOTA: Se desejar limpar folgas anteriores, descomente as linhas abaixo
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

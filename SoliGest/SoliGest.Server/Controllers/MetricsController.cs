using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

namespace SoliGest.Server.Controllers
{
    /// <summary>
    /// Controlador responsável por fornecer métricas do sistema.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly SoliGestServerContext _context;

        /// <summary>
        /// Construtor do controlador de métricas.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        public MetricsController(SoliGestServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém o número total de utilizadores registados.
        /// </summary>
        /// <returns>Número total de utilizadores.</returns>
        [HttpGet("total-usuarios")]
        public async Task<IActionResult> GetTotalUsers()
        {
            int total = await _context.Users.CountAsync();
            return Ok(new { totalUsers = total });
        }

        /// <summary>
        /// Obtém o número total de painéis solares registados.
        /// </summary>
        /// <returns>Número total de painéis solares.</returns>
        [HttpGet("total-paineis")]
        public async Task<IActionResult> GetTotalPainels()
        {
            int total = await _context.SolarPanel.CountAsync();
            return Ok(new { totalPanels = total });
        }

        /// <summary>
        /// Obtém o número total de pedidos de assistência.
        /// </summary>
        /// <returns>Número total de pedidos de assistência.</returns>
        [HttpGet("total-pedidos-assistencia")]
        public async Task<IActionResult> GetTotalAssistanceRequests()
        {
            int total = await _context.AssistanceRequest.CountAsync();
            return Ok(new { totalAssistanceRequests = total });
        }

        /// <summary>
        /// Obtém a quantidade de pedidos de assistência agrupados por prioridade.
        /// </summary>
        /// <returns>Dicionário contendo a prioridade e a respetiva quantidade.</returns>
        [HttpGet("avarias-priority")]
        public async Task<IActionResult> GetAssistanceRequestPerStatus()
        {
            var result = await _context.AssistanceRequest
                .GroupBy(a => a.Priority)
                .Select(g => new
                {
                    Prioridade = g.Key,
                    Quantidade = g.Count()
                })
                .ToListAsync();

            var dict = result.ToDictionary(x => x.Prioridade, x => x.Quantidade);

            return Ok(dict);
        }

        /// <summary>
        /// Obtém o tempo médio de reparação (em minutos) dos pedidos de assistência resolvidos.
        /// </summary>
        /// <returns>Tempo médio de reparação em minutos.</returns>
        [HttpGet("tempo-medio-reparacao")]
        public async Task<IActionResult> GetAverageRepairTime()
        {
            var requests = await _context.AssistanceRequest
                .Where(a => !string.IsNullOrEmpty(a.ResolutionDate))
                .ToListAsync();

            var repairTimes = requests.Select(f =>
            {
                bool openParsed = DateTime.TryParse(f.RequestDate, out DateTime openDate);
                bool closeParsed = DateTime.TryParse(f.ResolutionDate, out DateTime closeDate);

                if (openParsed && closeParsed)
                {
                    return (closeDate - openDate).TotalMinutes;
                }
                return (double?)null;
            })
            .Where(time => time.HasValue)
            .Select(time => time.Value)
            .ToList();

            double averageRepairTime = repairTimes.Any() ? repairTimes.Average() : 0;
            return Ok(new { averageRepairTime });
        }
    }
}

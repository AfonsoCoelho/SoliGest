using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

[Route("api/[controller]")]
[ApiController]
public class MetricsController : ControllerBase
{
    private readonly SoliGestServerContext _context;

    public MetricsController(SoliGestServerContext context)
    {
        _context = context;
    }

    [HttpGet("total-usuarios")]
    public async Task<IActionResult> GetTotalUsers()
    {
        int total = await _context.Users.CountAsync();
        return Ok(new { totalUsers = total });
    }

    [HttpGet("total-paineis")]
    public async Task<IActionResult> GetTotalPainels()
    {
        int total = await _context.SolarPanel.CountAsync();
        return Ok(new { totalPanels = total });
    }



    [HttpGet("avarias-status")]
    public async Task<IActionResult> GetAssistanceRequestPerStatus()
    {
        var result = await _context.AssistanceRequest
            .GroupBy(a => a.Status)
            .Select(g => new
            {
                Status = g.Key,
                Quantidade = g.Count()
            })
            .ToListAsync();

        var dict = result.ToDictionary(x => x.Status, x => x.Quantidade);

        return Ok(dict);
    }

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

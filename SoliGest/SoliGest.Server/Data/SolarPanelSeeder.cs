using Microsoft.AspNetCore.Identity;
using SoliGest.Server.Models;
using SoliGest.Server.Data;
using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Data
{
    public class SolarPanelSeeder
    {
        public static async Task SeedSolarPanelsAsync(SoliGestServerContext context)
        {
            SolarPanel solarPanel1 = new SolarPanel
            {
                Name = "Solar Panel 1",
                Priority = "Alta",
                Status = "Em curso",
                StatusClass = "Em curso",
                Latitude = 8,
                Longitude = 8,
                Description = "999999999",
                PhoneNumber = 8,
                Email = "Sábado",
                Address = "2025-06-01"
            };

            await context.AddAsync<SolarPanel>(solarPanel1);

            SolarPanel solarPanel2 = new SolarPanel
            {
                Name = "Solar Panel 2",
                Priority = "Alta",
                Status = "Em curso",
                StatusClass = "Em curso",
                Latitude = 8,
                Longitude = 8,
                Description = "999999999",
                PhoneNumber = 8,
                Email = "Sábado",
                Address = "2025-06-01"
            };

            await context.AddAsync<SolarPanel>(solarPanel2);

            SolarPanel solarPanel3 = new SolarPanel
            {
                Name = "Solar Panel 3",
                Priority = "Alta",
                Status = "Em curso",
                StatusClass = "Em curso",
                Latitude = 8,
                Longitude = 8,
                Description = "999999999",
                PhoneNumber = 8,
                Email = "Sábado",
                Address = "2025-06-01"
            };

            await context.AddAsync<SolarPanel>(solarPanel3);

            await context.SaveChangesAsync();
        }
    }
}

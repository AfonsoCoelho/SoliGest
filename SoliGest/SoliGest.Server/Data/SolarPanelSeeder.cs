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
            if (await context.FindAsync<SolarPanel>(1) == null)
            {
                SolarPanel solarPanel1 = new SolarPanel
                {
                    Name = "Rua D. Afonso Henriques, Lisboa",
                    Priority = "Alta",
                    Status = "Vermelho",
                    StatusClass = "status-red",
                    Latitude = 38.7223,
                    Longitude = -9.1393,
                    Description = "Painel próximo ao centro",
                    PhoneNumber = 8,
                    Email = "contato@empresa.com",
                    Address = ""
                };

                await context.AddAsync(solarPanel1);

                SolarPanel solarPanel2 = new SolarPanel
                {
                    Name = "Avenida Principal, Almada",
                    Priority = "Média",
                    Status = "Verde",
                    StatusClass = "status-green",
                    Latitude = 38.6790,
                    Longitude = -9.1569,
                    Description = "Painel na filial sul",
                    PhoneNumber = 8,
                    Email = "suporte@empresa.com",
                    Address = ""
                };

                await context.AddAsync(solarPanel2);

                SolarPanel solarPanel3 = new SolarPanel
                {
                    Name = "Avenida Principal, Almada",
                    Priority = "Média",
                    Status = "Verde",
                    StatusClass = "status-green",
                    Latitude = 38.6790,
                    Longitude = -9.1569,
                    Description = "Painel na filial sul",
                    PhoneNumber = 8,
                    Email = "suporte@empresa.com",
                    Address = ""
                };

                await context.AddAsync(solarPanel3);

                await context.SaveChangesAsync();
            }
        }
    }
}

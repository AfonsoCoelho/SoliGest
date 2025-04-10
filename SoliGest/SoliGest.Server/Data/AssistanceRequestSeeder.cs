using Microsoft.AspNetCore.Identity;
using SoliGest.Server.Models;
using SoliGest.Server.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SoliGest.Server.Data
{
    public class AssistanceRequestSeeder
    {

        public static async Task SeedAssistanceRequestsAsync(SoliGestServerContext context)
        {
            var solarPanel1 = await context.SolarPanel.FirstOrDefaultAsync();
            var solarPanel2 = await context.SolarPanel.FirstOrDefaultAsync(a => a.Id == 2);
            var solarPanel3 = await context.SolarPanel.FirstOrDefaultAsync(a => a.Id == 3);

            if (!context.AssistanceRequest.Any())
            {
                AssistanceRequest assistanceRequest1 = new AssistanceRequest
                {
                    RequestDate = "2021-01-01",
                    Priority = "Alta",
                    Status = "Vermelho",
                    StatusClass = "status-red",
                    ResolutionDate = "2022-01-01",
                    Description = "Descrição Pedido de assistência 1",
                    SolarPanel = solarPanel1
                };

                await context.AddAsync(assistanceRequest1);

                AssistanceRequest assistanceRequest2 = new AssistanceRequest
                {
                    RequestDate = "2021-01-01",
                    Priority = "Média",
                    Status = "Vermelho",
                    StatusClass = "status-red",
                    ResolutionDate = "2022-01-01",
                    Description = "Descrição Pedido de assistência 2",
                    SolarPanel = solarPanel2
                };

                await context.AddAsync(assistanceRequest2);

                AssistanceRequest assistanceRequest3 = new AssistanceRequest
                {
                    RequestDate = "2021-01-01",
                    Priority = "Baixa",
                    Status = "Amarelo",
                    StatusClass = "status-yellow",
                    ResolutionDate = "2022-01-01",
                    Description = "Descrição Pedido de assistência 3",
                    SolarPanel = solarPanel3
                };

                await context.AddAsync(assistanceRequest3);

                await context.SaveChangesAsync();
            }
        }
    }
}

using Microsoft.AspNetCore.Identity;
using SoliGest.Server.Models;
using SoliGest.Server.Data;
using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Data
{
    public class AssistanceRequestSeeder
    {

        public static async Task SeedAssistanceRequestsAsync(SoliGestServerContext context)
        {
            var solarPanel1 = context.FindAsync<SolarPanel>(1).Result;

            if (await context.FindAsync<AssistanceRequest>(1) == null && solarPanel1 != null)
            {
                AssistanceRequest assistanceRequest1 = new AssistanceRequest
                {
                    RequestDate = "2021-01-01",
                    ResolutionDate = "2022-01-01",
                    Description = "Descrição Pedido de assistência 1",
                    SolarPanel = solarPanel1
                };

                await context.AddAsync(assistanceRequest1);

                AssistanceRequest assistanceRequest2 = new AssistanceRequest
                {
                    RequestDate = "2021-01-01",
                    ResolutionDate = "2022-01-01",
                    Description = "Descrição Pedido de assistência 2",
                    SolarPanel = solarPanel1
                };

                await context.AddAsync(assistanceRequest2);

                AssistanceRequest assistanceRequest3 = new AssistanceRequest
                {
                    RequestDate = "2021-01-01",
                    ResolutionDate = "2022-01-01",
                    Description = "Descrição Pedido de assistência 2",
                    SolarPanel = solarPanel1
                };

                await context.AddAsync(assistanceRequest3);

                await context.SaveChangesAsync();
            }
        }
    }
}

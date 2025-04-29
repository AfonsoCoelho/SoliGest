using Microsoft.AspNetCore.Identity;
using SoliGest.Server.Models;
using SoliGest.Server.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SoliGest.Server.Data
{
    /// <summary>
    /// Classe responsável por popular a tabela de pedidos de assistência com dados iniciais.
    /// </summary>
    public class AssistanceRequestSeeder
    {
        /// <summary>
        /// Método assíncrono para semear dados na tabela de pedidos de assistência.
        /// Verifica se já existem registros na tabela e, caso não haja, cria registros iniciais.
        /// </summary>
        /// <param name="context">O contexto do banco de dados que será utilizado para adicionar os registros.</param>
        /// <returns>Uma tarefa assíncrona representando a operação de semear os dados.</returns>
        public static async Task SeedAssistanceRequestsAsync(SoliGestServerContext context)
        {
            // Recupera o primeiro painel solar e outros específicos para os pedidos de assistência
            var solarPanel1 = await context.SolarPanel.FirstOrDefaultAsync();
            var solarPanel2 = await context.SolarPanel.FirstOrDefaultAsync(a => a.Id == 2);
            var solarPanel3 = await context.SolarPanel.FirstOrDefaultAsync(a => a.Id == 3);

            // Recupera um usuário com o papel de "Técnico"
            var user1 = await context.Users.FirstOrDefaultAsync(u => u.Role.Equals("Técnico"));

            // Verifica se o usuário técnico foi encontrado, caso contrário, exibe mensagem de alerta
            if (user1 == null)
            {
                Console.WriteLine("user1 e null atencao");
            }
            Console.WriteLine("teste yaa");

            // Verifica se já existem pedidos de assistência no banco de dados
            if (!context.AssistanceRequest.Any())
            {
                // Criação de um pedido de assistência
                AssistanceRequest assistanceRequest1 = new AssistanceRequest
                {
                    RequestDate = "2021-01-01",
                    Priority = "Alta",
                    Status = "Vermelho",
                    StatusClass = "status-red",
                    ResolutionDate = "2022-01-01",
                    Description = "Descrição Pedido de assistência 1",
                    SolarPanel = solarPanel1,
                    AssignedUser = user1
                };

                await context.AddAsync(assistanceRequest1);

                // Criação de outro pedido de assistência
                AssistanceRequest assistanceRequest2 = new AssistanceRequest
                {
                    RequestDate = "2021-01-01",
                    Priority = "Média",
                    Status = "Vermelho",
                    StatusClass = "status-red",
                    ResolutionDate = "2022-01-01",
                    Description = "Descrição Pedido de assistência 2",
                    SolarPanel = solarPanel2,
                    AssignedUser = user1
                };

                await context.AddAsync(assistanceRequest2);

                // Criação de um pedido de assistência com prioridade baixa
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

                // Salva as alterações no banco de dados
                await context.SaveChangesAsync();
            }
        }
    }
}

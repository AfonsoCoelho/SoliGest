using Microsoft.AspNetCore.Identity;
using SoliGest.Server.Models;
using SoliGest.Server.Data;
using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Data
{
    /// <summary>
    /// Classe responsável por popular a tabela de painéis solares com dados iniciais.
    /// </summary>
    public class SolarPanelSeeder
    {
        /// <summary>
        /// Método assíncrono para semear dados na tabela de painéis solares.
        /// Verifica se a tabela já contém registros, e se não, adiciona novos painéis solares.
        /// </summary>
        /// <param name="context">O contexto do banco de dados utilizado para interagir com as tabelas.</param>
        /// <returns>Uma tarefa assíncrona representando a operação de semear os painéis solares.</returns>
        public static async Task SeedSolarPanelsAsync(SoliGestServerContext context)
        {
            // Verifica se já existem registros de painéis solares no banco
            if (!context.SolarPanel.Any())
            {
                // Criação do primeiro painel solar
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
                    Address = "a"
                };

                await context.AddAsync(solarPanel1);

                // Criação do segundo painel solar
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
                    Address = "a"
                };

                await context.AddAsync(solarPanel2);

                // Criação do terceiro painel solar
                SolarPanel solarPanel3 = new SolarPanel
                {
                    Name = "Avenida Luisa Todi, Setúbal",
                    Priority = "Média",
                    Status = "Verde",
                    StatusClass = "status-green",
                    Latitude = 38.522882,
                    Longitude = -8.896155,
                    Description = "Painel na capital de distrito da margem sul",
                    PhoneNumber = 8,
                    Email = "luisa.todi@setubal.com",
                    Address = "a"
                };

                await context.AddAsync(solarPanel3);

                // Salva as alterações no banco de dados
                await context.SaveChangesAsync();
            }
        }
    }
}

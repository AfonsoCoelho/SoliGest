using Microsoft.AspNetCore.Identity;

namespace SoliGest.Server.Data
{
    /// <summary>
    /// Classe responsável por popular a tabela de papéis (roles) com dados iniciais.
    /// </summary>
    public class RoleSeeder
    {
        /// <summary>
        /// Método assíncrono para semear dados na tabela de papéis (roles).
        /// Verifica se os papéis já existem e, caso contrário, os cria.
        /// </summary>
        /// <param name="roleManager">O gerenciador de papéis (RoleManager) utilizado para criar os papéis.</param>
        /// <param name="logger">O logger utilizado para registrar erros durante a execução.</param>
        /// <returns>Uma tarefa assíncrona representando a operação de semear os papéis.</returns>
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager, ILogger<Program> logger)
        {
            // Lista de nomes de papéis que serão verificados e, se necessário, criados
            string[] roleNames = { "Supervisor", "Administrative", "Technician" };

            // Itera sobre cada nome de papel e verifica se o papel já existe
            foreach (var roleName in roleNames)
            {
                try
                {
                    // Se o papel não existe, cria um novo papel com o nome
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
                catch (Exception ex)
                {
                    // Em caso de erro, loga o erro no console e no logger
                    Console.WriteLine("RoleSeeder: " + ex);
                    logger.LogError("Role Seeder: " + ex);
                }
            }
        }
    }
}

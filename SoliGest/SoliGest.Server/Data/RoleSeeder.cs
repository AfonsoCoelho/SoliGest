using Microsoft.AspNetCore.Identity;

namespace SoliGest.Server.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Supervisor", "Administrative", "Technician" };
            foreach (var roleName in roleNames)
            {
                if(!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}

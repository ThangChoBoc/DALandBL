using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ZelnyTrh.EF.BL;
using ZelnyTrh.EF.DAL.Entities;

namespace ZelnyTrh.EF.DAL.Seeders;

public class RoleSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Seed roles
        string[] roleNames = [UserRoles.Administrator, UserRoles.Moderator, UserRoles.User];
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Seed admin user
        var adminEmail = "admin@zelnytrh.cz";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                Name = "System Administrator"
            };

            // Very bad
            // TODO: Fix this
            var result = await userManager.CreateAsync(admin, "Admin123!"); 
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, UserRoles.Administrator);
            }
        }
    }
}
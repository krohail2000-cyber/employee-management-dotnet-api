using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var db = services.GetRequiredService<AppDbContext>();
        await InitializeDatabaseAsync(db);

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                EnsureSucceeded(await roleManager.CreateAsync(new IdentityRole(role)));
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await EnsureUserAsync(userManager, "admin@employeeapi.com",
            configuration["SeedUsers:AdminPassword"], "Admin");
        await EnsureUserAsync(userManager, "user@employeeapi.com",
            configuration["SeedUsers:UserPassword"], "User");

        if (!await db.Departments.AnyAsync())
        {
            db.Departments.AddRange(
                new Department { Name = "IT", Description = "Information Technology" },
                new Department { Name = "HR", Description = "Human Resources" },
                new Department { Name = "Finance", Description = "Finance and Accounting" });
            await db.SaveChangesAsync();
        }
    }

    private static async Task InitializeDatabaseAsync(AppDbContext db)
    {
        await db.Database.OpenConnectionAsync();
        var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT VERSION()";
        var serverVersion = Convert.ToString(await command.ExecuteScalarAsync()) ?? string.Empty;
        await db.Database.CloseConnectionAsync();

        // Oracle's EF Core provider requests GET_LOCK with a -1 timeout. MariaDB
        // returns NULL for that call, which the provider cannot cast. EnsureCreated
        // avoids that provider-specific migration lock while preserving migrations
        // for MySQL servers and design-time SQL generation.
        if (serverVersion.Contains("MariaDB", StringComparison.OrdinalIgnoreCase))
        {
            await db.Database.EnsureCreatedAsync();
            return;
        }

        await db.Database.MigrateAsync();
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager, string email, string? password, string role)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException($"Seed password for {email} is not configured.");

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            EnsureSucceeded(await userManager.CreateAsync(user, password));
        }

        if (!await userManager.IsInRoleAsync(user, role))
            EnsureSucceeded(await userManager.AddToRoleAsync(user, role));
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
    }
}

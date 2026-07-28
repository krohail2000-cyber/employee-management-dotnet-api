using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EmployeeManagement.Api.Data;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=localhost;Port=3306;Database=EmployeeManagementDb;User=root;Password=;";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySQL(connectionString)
            .Options;
        return new AppDbContext(options);
    }
}

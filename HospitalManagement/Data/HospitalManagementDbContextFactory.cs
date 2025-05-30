using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using HospitalManagement.Data;

public class HospitalManagementDbContextFactory : IDesignTimeDbContextFactory<HospitalManagementDbContext>
{
    public HospitalManagementDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<HospitalManagementDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new HospitalManagementDbContext(optionsBuilder.Options);
    }
}

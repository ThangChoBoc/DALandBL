using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ZelnyTrh.EF.DAL
{
    public class ApplicationDbFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Determine the base path for the configuration
            // Assuming that the EF project is in 'ZelnyTrh.EF' and the 'appsettings.json' is in 'ZelnyTrh'
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\ZelnyTrh");

            // Ensure the path is absolute and normalized
            basePath = Path.GetFullPath(basePath);

            // Check if the 'appsettings.json' file exists at the computed path
            if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
            {
                throw new FileNotFoundException("Could not find 'appsettings.json' at path: " + basePath);
            }

            // Load configuration from 'appsettings.json'
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Retrieve the connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Configure DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
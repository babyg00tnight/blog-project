using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MyBlogCMS.Data
{
    // Factory này dùng cho lệnh "dotnet ef migrations"
    public class MyBlogContextFactory : IDesignTimeDbContextFactory<MyBlogContext>
    {
        public MyBlogContext CreateDbContext(string[] args)
        {
            // Load cấu hình từ appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<MyBlogContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new MyBlogContext(optionsBuilder.Options);
        }
    }
}

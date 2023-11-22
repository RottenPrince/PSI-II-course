using BrainBoxAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BrainBoxAPI.Tests.IntegrationTests;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
            );

            services.Remove(descriptor);
            var dbName = "TestDatabase_" + Guid.NewGuid().ToString();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite($"Data Source={dbName}.db");
            });

            services.AddScoped<AppDbContext>();

            var sp = services.BuildServiceProvider();


            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();

            
            
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.SeedTestData();
        });
    }
}

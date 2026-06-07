using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PracticeAssignment.Data;
using PracticeAssignment.Models;
using PracticeAssignment.API;


namespace Practice_assignment.Tests.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<ApiTestEntryPoint>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext configuration
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            // Add In-Memory database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            // Build and seed
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();

            // Seed data
            if (!db.Clients.Any())
            {
                db.Clients.Add(new Client { Id = 1, Name = "Test Client 1", ContactDetails = "test1@test.com", Region = "Africa" });
                db.SaveChanges();
            }
        });

        // Use Development environment to enable Swagger etc.
        builder.UseEnvironment("Development");
    }
}
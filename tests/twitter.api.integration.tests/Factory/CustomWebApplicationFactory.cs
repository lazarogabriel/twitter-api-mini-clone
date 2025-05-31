using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using twitter.api.data.DbContexts;

namespace twitter.api.integration.tests.Factory
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private SqliteConnection _connection;

        public CustomWebApplicationFactory()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // TODO: Encontrar la manera de remover esto correctamente.
                services.RemoveAll(typeof(DbContextOptions<TwitterApiDbContext>));
                services.RemoveAll(typeof(TwitterApiDbContext));
                services.RemoveAll(typeof(ITwitterApiDbContext));
                services.RemoveAll(typeof(IDbContextFactory<TwitterApiDbContext>));

                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                services.AddDbContext<TwitterApiDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                services.AddScoped<ITwitterApiDbContext>(sp => sp.GetRequiredService<TwitterApiDbContext>());

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<TwitterApiDbContext>();
                    db.Database.EnsureCreated();
                }
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _connection?.Dispose();
            }
        }
    }
}
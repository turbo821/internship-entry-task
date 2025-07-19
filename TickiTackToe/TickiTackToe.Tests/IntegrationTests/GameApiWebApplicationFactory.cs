
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TickiTackToe.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace TickiTackToe.Tests.IntegrationTests
{
    public class GameApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TickDbContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                var dbOptions = services.Where(
                    d => d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true).ToList();

                foreach (var descriptor in dbOptions)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<TickDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<TickDbContext>();
                    db.Database.EnsureCreated();
                }
            });

            return base.CreateHost(builder);
        }
    }
}

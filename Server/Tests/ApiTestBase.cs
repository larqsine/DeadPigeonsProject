using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using DataAccess;
using DataAccess.Models;
using System.Threading.Tasks;
using System.Linq;
using Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PgCtx;
using API;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tests
{
    public class ApiTestBase : WebApplicationFactory<Program>
    {
        private readonly PgCtxSetup<DBContext> pgctx;
        

        public ApiTestBase()
        {
            pgctx = new PgCtxSetup<DBContext>("postgres:latest");
        }
        
        public async Task Seed(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var ctx = scopedServices.GetRequiredService<DBContext>();
            var userManager = scopedServices.GetRequiredService<UserManager<User>>();

            
            await ctx.Database.EnsureCreatedAsync();

            // Seed admin and player users
            var admin = await TestObjects.GetAdmin(userManager);
            var player = await TestObjects.GetPlayer(userManager);

            // Save changes explicitly
            await ctx.SaveChangesAsync();
        }


        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContextOptions and add a new scoped DbContext
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DBContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Re-add DbContext scoped to the test environment
                services.AddDbContext<DBContext>(options =>
                    options.UseNpgsql(pgctx._postgres.GetConnectionString()));
                
            });

            var host = builder.Build();

            // Seed the database only after the host is created and services are available
            using (var scope = host.Services.CreateScope())
            {
                Seed(scope.ServiceProvider).GetAwaiter().GetResult();
            }

            return host;
        }
        
    }
}
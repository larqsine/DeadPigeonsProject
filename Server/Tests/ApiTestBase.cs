using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using DataAccess;
using DataAccess.Models;
using System.Threading.Tasks;
using System.Linq;
using ApiInterationTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PgCtx;
using API;

namespace Tests
{
    public class ApiTestBase : WebApplicationFactory<Program>
    {
        private readonly PgCtxSetup<DBContext> pgctx;
        

        // Initialize the environment and set up required services
        public ApiTestBase()
        {
            
            
            pgctx = new PgCtxSetup<DBContext>("postgres:latest");

            
        }
        

        public async Task Seed(IServiceProvider services)
        {
            var ctx = services.GetRequiredService<DataAccess.DBContext>();

            // Seed default data for tests
            var userManager = services.GetRequiredService<UserManager<User>>();
            var admin = await TestObjects.GetAdmin(userManager);
            var player = await TestObjects.GetPlayer(userManager);
            
            var game = TestObjects.GetGame(admin);
            var board = TestObjects.GetBoard(player, game);
            var transaction = TestObjects.GetTransaction(player);
            var winner = TestObjects.GetWinner(game, player, board);

            /*
            ctx.Admins.Add(admin);
            ctx.Players.Add(player);
            */
            ctx.Games.Add(game);
            ctx.Boards.Add(board);
            ctx.Transactions.Add(transaction);
            ctx.Winners.Add(winner);
        
            

            await ctx.SaveChangesAsync();
            
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Remove the existing DBContext setup
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContextOptions
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DBContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Use the PostgreSQL connection string from PgCtx
                services.AddDbContext<DBContext>(options =>
                    options.UseNpgsql(pgctx._postgres.GetConnectionString()));
                var constring = pgctx._postgres.GetConnectionString();
                Console.WriteLine(constring);
            });

            // Build the host
            var host = builder.Build();

            // Seed the database (initialize services before starting)
            using (var scope = host.Services.CreateScope())
            {
                Seed(scope.ServiceProvider).GetAwaiter().GetResult();
            }
            

            return host;
        }

    }
}

using E_Commerce.Domain.Contracts;
using E_Commerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ECommerceWeb.Extensions
{
    public static class WebApplicationRegisteration
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {

            await using var Scope = app.Services.CreateAsyncScope();

            var DbContextService = Scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            var PendingMigrations = await DbContextService.Database.GetPendingMigrationsAsync();
            if (PendingMigrations.Any())
                await DbContextService.Database.MigrateAsync();

            return app;

        }
        public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
        {
            await using var Scope = app.Services.CreateAsyncScope();
            var dbContextService = Scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            var DataIntializerService = Scope.ServiceProvider.GetRequiredService<IDataIntializer>();
            await DataIntializerService.IntializeAsync();

            return app;
        }
    }
}

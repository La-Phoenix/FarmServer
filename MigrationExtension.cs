using FarmServer.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FarmServer
{
    public static class MigrationExtension
    {
        public static void ApplyMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<IApplicationBuilder>>();

            try
            {
                var dbContext = services.GetRequiredService<FarmDbContext>();
                dbContext.Database.Migrate();  // Apply pending migrations
                logger.LogInformation("Database migration applied successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while applying database migrations.");
            }
        }
    }
}

using FarmServer.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FarmServer
{
    public static class MigrationExtension
    {
        public static void ApplyMigration(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using FarmDbContext dbContext = 
                scope.ServiceProvider.GetRequiredService<FarmDbContext>();

            dbContext.Database.Migrate();
        }
    }
}

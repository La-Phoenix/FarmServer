using FarmServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmServer.Infrastructure
{
    public class FarmDbContext: DbContext
    {
        public FarmDbContext(DbContextOptions<FarmDbContext> options) : base(options) { }

        public DbSet<Farm> Farms { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<Farmer> Farmers { get; set; }
    }
}

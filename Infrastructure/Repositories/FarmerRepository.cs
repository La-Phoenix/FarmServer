using FarmServer.Domain.Entities;
using FarmServer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmServer.Infrastructure.Repositories
{
    public class FarmerRepository: IFarmerRepository
    {
        private readonly FarmDbContext context;

        public FarmerRepository(FarmDbContext context)
        {
            this.context = context;
        }

        public async Task<Farmer> AddAsync(Farmer farmer)
        {
            context.Farmers.Add(farmer);
            await context.SaveChangesAsync();
            return farmer;
        }

        public async Task DeleteAsync(Guid id)
        {
            var farmer = await context.Farmers.FindAsync(id);
            if(farmer != null)
            {
                context.Farmers.Remove(farmer);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Farmer>> GetAllAsync()
        {
            return await context.Farmers.
                Include(f => f.Farms)  //Eager Loading - mtell EF Core to include related data.
                .ToListAsync();
        }

        public async Task<Farmer?> GetByIdAsync(Guid id)
        {
            return await context.Farmers
                .Include(f => f.Farms).FirstOrDefaultAsync(f => f.Id == id);
        }

        // This method is used to mark the entity as modified so that it can be updated in the database
        public void MarkAsModified(Farmer farmer)
        {
            context.Entry(farmer).State = EntityState.Modified;
        }

        // This method is used to save the changes to the database
        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }


        public async Task<Farmer> UpdateAsync(Farmer farmer)
        {
            context.Farmers.Update(farmer);
            await context.SaveChangesAsync();
            return farmer;

        }
    }
}

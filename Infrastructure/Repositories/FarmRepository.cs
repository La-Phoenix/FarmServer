using FarmServer.Domain.Entities;
using FarmServer.Infrastructure;
using FarmServer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmServer.Infrastructure.Repositories
{
    public class FarmRepository: IFarmRepository
    {
        private readonly FarmDbContext context;

        public FarmRepository(FarmDbContext context) 
        {
            this.context = context;
        }

        public async Task<IEnumerable<Farm>> GetAllAsync()
        {
            return await context.Farms.ToListAsync();
        }

        public async Task<Farm?> GetByIdAsync(Guid id)
        {
            return await context.Farms.FindAsync(id);
        }

        public async Task<Farm> AddAsync(Farm farm)
        {
            context.Farms.Add(farm);
            await context.SaveChangesAsync();
            return farm;
        }

        public async Task<Farm> UpdateAsync(Farm farm)
        {
            context.Farms.Update(farm);
            await context.SaveChangesAsync();
            return farm;
        }

        public async Task DeleteAsync(Guid id)
        {
            var farm = await context.Farms.FindAsync(id);
            if (farm != null)
            {
                context.Farms.Remove(farm);
                await context.SaveChangesAsync();
            }
        }
    }
}

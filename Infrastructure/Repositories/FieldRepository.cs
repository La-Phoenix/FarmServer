using FarmServer.Domain.Entities;
using FarmServer.Interfaces.IField;
using Microsoft.EntityFrameworkCore;

namespace FarmServer.Infrastructure.Repositories
{
    public class FieldRepository: IFieldRepository
    {
        private readonly FarmDbContext context;

        public FieldRepository(FarmDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Field>> GetAllAsync()
        {
            return await context.Fields.
                Include(f => f.Farm).
                ToListAsync();
        }

        public async Task<Field?> GetByIdAsync(Guid id)
        {
            return await context.Fields
                .Include(f => f.Farm).FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Field> AddAsync(Field field)
        {
            context.Fields.Add(field);
            await context.SaveChangesAsync();
            return field;
        }

        public async Task DeleteAsync(Guid id)
        {
            var field = await context.Fields.FindAsync(id);
            if (field != null)
            {
                context.Fields.Remove(field);
                await context.SaveChangesAsync();
            }
        }

        public void MarkAsModified(Field field)
        {
            context.Entry(field).State = EntityState.Modified;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<Field> UpdateAsync(Field field)
        {
            context.Fields.Update(field);
            await context.SaveChangesAsync();
            return field;
        }
    }
}

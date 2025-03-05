

using FarmServer.Domain.Entities;

namespace FarmServer.Interfaces.IField
{
    public interface IFieldRepository
    {
        Task<IEnumerable<Field>> GetAllAsync();
        Task<Field?> GetByIdAsync(Guid id);
        Task<Field> AddAsync(Field field);
        Task<Field> UpdateAsync(Field field);
        Task DeleteAsync(Guid id);
        Task SaveAsync();
        void MarkAsModified(Field field);
    }
}

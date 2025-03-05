using FarmServer.Domain.Entities;

namespace FarmServer.Interfaces
{
    public interface IFarmerRepository
    {
        Task<IEnumerable<Farmer>> GetAllAsync();
        Task<Farmer?> GetByIdAsync(Guid id);
        Task<Farmer?> GetByEmailAsync(string email);
        Task<Farmer> AddAsync(Farmer farmer);
        Task<Farmer> UpdateAsync(Farmer farmer);
        Task DeleteAsync(Guid id);
        void MarkAsModified(Farmer farmer);
        Task SaveAsync();
    }
}

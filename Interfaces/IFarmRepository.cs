using FarmServer.Domain.Entities;

namespace FarmServer.Interfaces
{
    public interface IFarmRepository
    {
        Task<IEnumerable<Farm>> GetAllAsync();
        Task<Farm?> GetByIdAsync(Guid id);
        Task<Farm> AddAsync(Farm farm);
        Task<Farm> UpdateAsync(Farm farm);
        Task DeleteAsync(Guid id);
    }
}

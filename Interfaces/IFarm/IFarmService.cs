using FarmServer.DTOs.Farm;

namespace FarmServer.Interfaces.IFarm
{
    public interface IFarmService
    {
        Task<IEnumerable<FarmDTO>> GetAllAsync();
        Task<FarmDTO?> GetByIdAsync(Guid id);
        Task<FarmDTO> CreateAsync(CreateFarmDTO farmDto);
        Task<FarmDTO?> UpdateAsync(Guid id, UpdateFarmDTO farmDto);
        Task<bool> DeleteAsync(Guid id);
    }
}

using FarmServer.DTOs.Farmer;

namespace FarmServer.Interfaces
{
    public interface IFarmerService
    {
        Task<IEnumerable<FarmerDTO>> GetAllAsync();
        Task<FarmerDTO?> GetByIdAsync(Guid id);
        Task<FarmerDTO> CreateAsync(CreateFarmerDTO farmerDto);
        Task<FarmerDTO?> UpdateAsync(Guid id, UpdateFarmerDTO farmerDto);
        Task<bool> DeleteAsync(Guid id);

    }
}

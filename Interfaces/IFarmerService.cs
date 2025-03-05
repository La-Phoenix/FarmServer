﻿using FarmServer.DTOs.Farmer;

namespace FarmServer.Interfaces
{
    public interface IFarmerService
    {
        Task<IEnumerable<FarmerDTO>> GetAllAsync();
        Task<FarmerDTO?> GetByIdAsync(Guid id);
        Task<FarmerDTO?> GetByEmailAsync(string email);
        Task<FarmerDTO> CreateAsync(CreateFarmerDTO farmerDto);
        Task<FarmerDTO?> UpdateAsync(Guid id, PartialUpdateFarmerDTO farmerDto);
        Task<bool> DeleteAsync(Guid id);

    }
}

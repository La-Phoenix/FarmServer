using FarmServer.DTOs.Field;

namespace FarmServer.Interfaces.IField
{
    public interface IFieldService
    {
        Task<IEnumerable<FieldDTO>> GetAllAsync();
        Task<FieldDTO?> GetByIdAsync(Guid id);
        Task<FieldDTO> CreateAsync(CreateFieldDTO fieldDTO);
        Task<FieldDTO?> UpdateAsync(Guid id, UpdateFieldDTO fieldDTO);
        Task<bool> DeleteAsync(Guid id);
    }
}

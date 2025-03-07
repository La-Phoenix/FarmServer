using FarmServer.Domain.Entities;
using FarmServer.DTOs.Farm;
using FarmServer.DTOs.Field;
using FarmServer.Interfaces.IFarm;
using FarmServer.Interfaces.IField;

namespace FarmServer.Infrastructure.Services
{
    public class FieldService: IFieldService
    {
        private readonly IFieldRepository fieldRepository;
        private readonly IFarmRepository farmRepository;

        public FieldService(IFieldRepository fieldRepository, IFarmRepository farmRepository)
        {
            this.fieldRepository = fieldRepository;
            this.farmRepository = farmRepository;
        }

        public async Task<FieldDTO?> CreateAsync(CreateFieldDTO fieldDTO)
        {
            var farm = await farmRepository.GetByIdAsync(fieldDTO.FarmId);
            if (farm == null) return null;
            var id = Guid.NewGuid();
            var field = new Field
            {
                Id = id,
                Name = fieldDTO.Name,
                FarmId = fieldDTO.FarmId,
                CropType = fieldDTO.CropType,
                Area = fieldDTO.Area,
                //Farm = farm
            };
            await fieldRepository.AddAsync(field);

            return new FieldDTO
            {
                Id = field.Id,
                Name = field.Name,
                CropType = field.CropType,
                Area = field.Area,
                FarmId = field.FarmId,
                Farm = new PartialFarmUpdateDTO
                {
                    Id = farm.Id,
                    Name = farm.Name,
                    Location = farm.Location
                }
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var field = fieldRepository.GetByIdAsync(id);
            if (field == null) return false;
            await fieldRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<FieldDTO>> GetAllAsync()
        {
            var fields = await fieldRepository.GetAllAsync();
            if (!fields.Any())
            {
                return new List<FieldDTO>();
            }

            return fields.Select(field => new FieldDTO
            {
                Id = field.Id,
                Name = field.Name,
                FarmId = field.FarmId,
                CropType = field.CropType,
                Area = field.Area,
                Farm = new PartialFarmUpdateDTO
                {
                    Id = field.Farm.Id,
                    Name = field.Farm.Name,
                    Location = field.Farm.Location
                }
            }).ToList();
        }

        public async Task<FieldDTO?> GetByIdAsync(Guid id)
        {
            var field = await fieldRepository.GetByIdAsync(id);
            if (field == null) return null;
            return new FieldDTO
            {
                Id = field.Id,
                Name = field.Name,
                FarmId = field.FarmId,
                CropType = field.CropType,
                Area = field.Area,
                Farm = new PartialFarmUpdateDTO
                {
                    Id = field.Farm.Id,
                    Name = field.Farm.Name,
                    Location = field.Farm.Location
                }
            };
        }

        public async Task<FieldDTO?> UpdateAsync(Guid id, UpdateFieldDTO fieldDTO)
        {
            var field = await fieldRepository.GetByIdAsync(id);
            if (field == null) return null;

            // Update Simple Properties
            if (!string.IsNullOrEmpty(fieldDTO.Name)) field.Name = fieldDTO.Name;
            if (!string.IsNullOrEmpty(fieldDTO.CropType)) field.CropType = fieldDTO.CropType;
            if (fieldDTO.Area.HasValue) field.Area = fieldDTO.Area.Value;
            if (fieldDTO.FarmId.HasValue) field.FarmId = fieldDTO.FarmId.Value;



            await fieldRepository.UpdateAsync(field);

            return new FieldDTO
            {
                Id = field.Id,
                Name = field.Name,
                FarmId = field.FarmId,
                CropType = field.CropType,
                Area = field.Area,
                Farm = new PartialFarmUpdateDTO
                {
                    Id = field.Farm.Id,
                    Name = field.Farm.Name,
                    Location = field.Farm.Location
                }
            };
        }
    }
}

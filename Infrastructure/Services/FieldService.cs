using AutoMapper;
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
        private readonly IMapper mapper;

        public FieldService(IFieldRepository fieldRepository, IFarmRepository farmRepository, IMapper mapper)
        {
            this.fieldRepository = fieldRepository;
            this.farmRepository = farmRepository;
            this.mapper = mapper;
        }

        public async Task<FieldDTO?> CreateAsync(CreateFieldDTO fieldDTO)
        {
            var farm = await farmRepository.GetByIdAsync(fieldDTO.FarmId);
            if (farm == null) return null;

            var field = mapper.Map<Field>(fieldDTO); // Automapper maps the properties of the fieldDTO to the field entity and assigns Id to a new Guid
            await fieldRepository.AddAsync(field);

            return mapper.Map<FieldDTO>(field);
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

            return mapper.Map<IEnumerable<FieldDTO>>(fields);
        }

        public async Task<FieldDTO?> GetByIdAsync(Guid id)
        {
            var field = await fieldRepository.GetByIdAsync(id);
            if (field == null) return null;
            return mapper.Map<FieldDTO>(field);
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

            return mapper.Map<FieldDTO>(field);
        }
    }
}

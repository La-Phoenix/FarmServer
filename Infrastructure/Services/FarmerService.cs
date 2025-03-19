using AutoMapper;
using FarmServer.Domain.Entities;
using FarmServer.DTOs.Farm;
using FarmServer.DTOs.Farmer;
using FarmServer.Interfaces.IFarm;
using FarmServer.Interfaces.IFarmer;

namespace FarmServer.Infrastructure.Services
{
    public class FarmerService : IFarmerService
    {
        private readonly IFarmerRepository farmerRepository;
        private readonly IFarmRepository farmRepository;
        private readonly IMapper mapper;

        public FarmerService(IFarmerRepository farmerRepository, IFarmRepository farmRepository, IMapper mapper)
        {
            this.farmerRepository = farmerRepository;
            this.farmRepository = farmRepository;
            this.mapper = mapper;
        }

        public async Task<FarmerDTO?> Login(FarmerLoginDTO farmerDto)
        {

            var farmer = await farmerRepository.GetByEmailAsync(farmerDto.Email);
            if (farmer == null) return null;
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(farmerDto.Password, farmer.Password);
            if (!isPasswordValid) return null;

            return mapper.Map<FarmerDTO>(farmer);
        }

        public async Task<FarmerDTO> CreateAsync(CreateFarmerDTO farmerDto)
        {
            var farmerToAdd = mapper.Map<Farmer>(farmerDto);

            var farmer = await farmerRepository.AddAsync(farmerToAdd);

            return mapper.Map<FarmerDTO>(farmer);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var farmer = await farmerRepository.GetByIdAsync(id);
            if (farmer == null) return false;
            await farmerRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<FarmerDTO>> GetAllAsync()
        {
            var farmers = await farmerRepository.GetAllAsync();
            if (!farmers.Any())
            {
                return new List<FarmerDTO>();
            }

            return mapper.Map<IEnumerable<FarmerDTO>>(farmers);
        }

        public async Task<FarmerDTO?> GetByIdAsync(Guid id)
        {
            var farmer = await farmerRepository.GetByIdAsync(id);
            if (farmer == null) return null;
            return mapper.Map<FarmerDTO>(farmer);
        }

        public async Task<FarmerDTO?> GetByEmailAsync(string email)
        {
            var farmer = await farmerRepository.GetByEmailAsync(email);
            if (farmer == null) return null;
            return mapper.Map<FarmerDTO>(farmer);
        }

        public async Task<FarmerDTO?> UpdateAsync(Guid id, PartialUpdateFarmerDTO farmerDto)
        {
            var farmer = await farmerRepository.GetByIdAsync(id);
            if (farmer == null) return null;

            // Update simple properties
            if (!string.IsNullOrEmpty(farmerDto.Name)) farmer.Name = farmerDto.Name;
            if (!string.IsNullOrEmpty(farmerDto.Location)) farmer.Location = farmerDto.Location;

            var existingFarmId = farmer.Farms.Select(farm => farm.Id);
            var updatedFarmIds = farmerDto.Farms.Select(farm => farm.Id);

            // add farms that are not already in the farmer's farm list
            var farmsToAdd = updatedFarmIds.Except(existingFarmId);

            foreach (var farmId in farmsToAdd)
            {
                // To get the farmToAdd details from the farmerDto
                var farmDto = farmerDto.Farms.FirstOrDefault(f => f.Id == farmId);
                if (farmDto != null)
                {
                    // Check if the farm already exists in the database
                    var farm = await farmRepository.GetByIdAsync(farmId);
                    if (farm != null)
                    {
                        farmer.Farms.Add(farm);
                    }
                    else
                    {
                        var newFarm = mapper.Map<Farm>(farmDto);
                        await farmRepository.AddAsync(newFarm);

                        farmer.Farms.Add(newFarm);
                    }
                }
            }

            // Update existing farms in the farmer's farm list
            foreach (var farm in farmer.Farms.ToList())
            {
                // To get farmToUpdate details from the farmerDto
                var updatedFarm = farmerDto.Farms.FirstOrDefault(f => f.Id == farm.Id);
                if (updatedFarm != null)
                {
                    // Upadate simple properties
                    if (!string.IsNullOrEmpty(updatedFarm.Name)) farm.Name = updatedFarm.Name;
                    if (!string.IsNullOrEmpty(updatedFarm.Location)) farm.Location = updatedFarm.Location;


                    // Mark the farm as modified so that it can be updated in the database
                    farmRepository.MarkAsModified(farm);
                }
            }

            await farmerRepository.UpdateAsync(farmer);
            // Save the farm changes to the database
            await farmRepository.SaveAsync();

            return mapper.Map<FarmerDTO>(farmer);
        }
    }
}

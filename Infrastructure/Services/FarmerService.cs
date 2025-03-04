using FarmServer.Domain.Entities;
using FarmServer.DTOs.Farm;
using FarmServer.DTOs.Farmer;
using FarmServer.Interfaces;

namespace FarmServer.Infrastructure.Services
{
    public class FarmerService : IFarmerService
    {
        private readonly IFarmerRepository farmerRepository;
        private readonly IFarmRepository farmRepository;

        public FarmerService(IFarmerRepository farmerRepository, IFarmRepository farmRepository)
        {
            this.farmerRepository = farmerRepository;
            this.farmRepository = farmRepository;
        }

        public async Task<FarmerDTO> CreateAsync(CreateFarmerDTO farmerDto)
        {
            var id = Guid.NewGuid();
            var farmer = new Farmer
            {
                Id = id,
                Name = farmerDto.Name,
                Email = farmerDto.Email,
                Location = farmerDto.Location
            };

            await farmerRepository.AddAsync(farmer);

            return new FarmerDTO
            {
                Id = farmer.Id,
                Name = farmer.Name,
                Email = farmer.Email,
                Location = farmer.Location
            };
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

            return farmers.Select(farmer => new FarmerDTO
            {
                Id = farmer.Id,
                Name = farmer.Name,
                Email = farmer.Email,
                Location = farmer.Location,
                Farms = farmer.Farms.Select(farm => new FarmDTO
                {
                    Id = farm.Id,
                    Name = farm.Name,
                    Location = farm.Location
                }).ToList()
            });
        }

        public async Task<FarmerDTO?> GetByIdAsync(Guid id)
        {
            var farmer = await farmerRepository.GetByIdAsync(id);
            if (farmer == null) return null;
            return new FarmerDTO
            {
                Id = farmer.Id,
                Name = farmer.Name,
                Email = farmer.Email,
                Location = farmer.Location,
                Farms = farmer.Farms.Select(farm => new FarmDTO
                {
                    Id = farm.Id,
                    Name = farm.Name,
                    Location = farm.Location
                }).ToList()
            };
        }

        public async Task<FarmerDTO?> UpdateAsync(Guid id, UpdateFarmerDTO farmerDto)
        {
            var farmer = await farmerRepository.GetByIdAsync(id);
            if (farmer == null) return null;

            // Update simple properties
            if (!string.IsNullOrEmpty(farmerDto.Name)) farmer.Name = farmerDto.Name;
            if (!string.IsNullOrEmpty(farmerDto.Email)) farmer.Email = farmerDto.Email;
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
                        var newFarm = new Farm
                        {
                            Id = new Guid(),
                            Name = farmDto.Name,
                            Location = farmDto.Location
                        };
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

            return new FarmerDTO
            {
                Id = farmer.Id,
                Name = farmer.Name,
                Email = farmer.Email,
                Location = farmer.Location,
                Farms = farmer.Farms.Select(farm => new FarmDTO
                {
                    Id = farm.Id,
                    Name = farm.Name,
                    Location = farm.Location
                }).ToList()
            };
        }
    }
}

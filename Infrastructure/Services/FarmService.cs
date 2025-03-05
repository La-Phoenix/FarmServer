using FarmServer.Domain.Entities;
using FarmServer.DTOs;
using FarmServer.DTOs.Farm;
using FarmServer.DTOs.Farmer;
using FarmServer.Interfaces.IFarm;
using FarmServer.Interfaces.IFarmer;

namespace FarmServer.Infrastructure.Services
{
    public class FarmService : IFarmService
    {
        private readonly IFarmRepository farmRepository;
        private readonly IFarmerRepository farmerRepository;

        public FarmService(IFarmRepository farmRepository, IFarmerRepository farmerRepository) 
        {
            this.farmRepository = farmRepository;
            this.farmerRepository = farmerRepository;
        }

        public async Task<IEnumerable<FarmDTO>> GetAllAsync()
        {
            var farms = await farmRepository.GetAllAsync();
            // Convert farms to JSON for better logging
            //Console.WriteLine(JsonSerializer.Serialize(farms, new JsonSerializerOptions { WriteIndented = true }));

            if (!farms.Any())
            {
                return [];
            }

            return farms.Select(farm => new FarmDTO
            {
                Id = farm.Id,
                Name = farm.Name,
                Location = farm.Location,
                Farmers = farm.Farmers.Select(farmer => new FarmerDTO
                {
                    Id = farmer.Id,
                    Name = farmer.Name,
                    Email = farmer.Email,
                    Location = farmer.Location
                }).ToList(),
                Fields = farm.Fields.Select(field => new FieldDTO
                {
                    Id = field.Id,
                    Name = field.Name,
                    CropType = field.CropType,
                    Area = field.Area
                }).ToList()
            });
        }

        public async Task<FarmDTO?> GetByIdAsync(Guid id)
        {
            var farm = await farmRepository.GetByIdAsync(id);
            Console.WriteLine(farm);

            if (farm == null) return null;

            return new FarmDTO
            {
                Id = farm.Id,
                Name = farm.Name,
                Location = farm.Location,
                Farmers = farm.Farmers.Select(farmer => new FarmerDTO
                {
                    Id = farmer.Id,
                    Name = farmer.Name,
                    Email = farmer.Email,
                    Location = farmer.Location
                }).ToList(),
                Fields = farm.Fields.Select(field => new FieldDTO
                {
                    Id = field.Id,
                    Name = field.Name,
                    CropType = field.CropType,
                    Area = field.Area
                }).ToList()
            };

        }

        public async Task<FarmDTO> CreateAsync(CreateFarmDTO farmDTO)
        {
            var id = Guid.NewGuid();
            var farm = new Farm
            {
                Id = id,
                Name = farmDTO.Name,
                Location = farmDTO.Location
            };

            await farmRepository.AddAsync(farm);

            return new FarmDTO
            {
                Id = farm.Id,
                Name = farm.Name,
                Location = farm.Location
            };
        }



        // Update Farm
        public async Task<FarmDTO?> UpdateAsync(Guid id, UpdateFarmDTO farmDTO)
        {
            var farm = await farmRepository.GetByIdAsync(id);

            if (farm == null) return null;

            // Update simple properties
            if (!string.IsNullOrEmpty(farmDTO.Name)) farm.Name = farmDTO.Name;
            if (!string.IsNullOrEmpty(farmDTO.Location)) farm.Location = farmDTO.Location;

            ////Get list of ids for fields in request farm and database farm
            //var existingFieldIds = farm.Fields.Select(f => f.Id).ToList();
            //var updatedFieldIds = farmDTO.Fields.Select(f => f.Id).ToList();

            //// Add new fields that are not already in the farm's field list
            //var fieldsToAdd = updatedFieldIds.Except(existingFieldIds);

            //foreach (var fieldId in fieldsToAdd)
            //{
            //    // To get the fieldToAdd details from the farmDTO
            //    var fieldDTO = farmDTO.Fields.FirstOrDefault(f => f.Id == fieldId);
            //    if (fieldDTO != null)
            //    {
            //        // Check if the field already exists in the database
            //        var field = await fieldRepository.GetByIdAsync(fieldId);
            //        if (field != null)
            //        {
            //            farm.Fields.Add(field);
            //        }
            //        else
            //        {
            //            var newField = new Field
            //            {
            //                Id = Guid.NewGuid(),,
            //                Name = fieldDTO.Name,
            //                CropType = fieldDTO.CropType,
            //                Area = fieldDTO.Area,
            //                FarmId = farm.Id
            //            };
            //            await fieldRepository.AddAsync(newField);
            //            farm.Fields.Add(newField);

            //        }
            //    }
            //}

            //// Update existing fields in the farm's field list
            //foreach (var field in farm.Fields.ToList())
            //{
            //    // To get the fieldToUpdate details from the farmDTO
            //    var updatedField = farmDTO.Fields.FirstOrDefault(f => f.Id == field.Id);
            //    if (updatedField != null)
            //    {
            //        // Update simple properties
            //        if (!string.IsNullOrEmpty(updatedField.Name)) field.Name = updatedField.Name;
            //        if (!string.IsNullOrEmpty(updatedField.CropType)) field.CropType = updatedField.CropType;
            //        if (updatedField.Area != 0) field.Area = updatedField.Area;
            //    }
            //}

            // Get list of emails for farmers in request farm and database farm
            var existingFarmerEmails = farm.Farmers.Select(f => f.Email).ToList();
            var updatedFarmerEmails = farmDTO.Farmers.Select(f => f.Email).ToList();

            // Add new farmers that are not already in the farm's farmer list
            var farmersToAdd = updatedFarmerEmails.Except(existingFarmerEmails);

            foreach (var farmerEmail in farmersToAdd)
            {
                // To get the farmerToAdd details from the farmDTO
                var farmerDTO = farmDTO.Farmers.FirstOrDefault(f => f.Email == farmerEmail);
                if (farmerDTO != null)
                {
                    // Check if the farmer already exists in the database
                    var farmer = await farmerRepository.GetByEmailAsync(farmerEmail);
                    if (farmer != null)
                    {
                        farm.Farmers.Add(farmer);
                    }
                    else
                    {
                        var newFarmer = new Farmer
                        {
                            Id = Guid.NewGuid(),
                            Name = farmerDTO.Name ?? string.Empty, // Ensure Name is not null
                            Email = farmerDTO.Email,
                            Location = farmerDTO.Location ?? string.Empty // Ensure Location is not null
                        };
                        await farmerRepository.AddAsync(newFarmer);
                        farm.Farmers.Add(newFarmer);
                    }
                }
            }

            // Update existing farmers in the farm's farmer list
            foreach (var farmer in farm.Farmers.ToList())
            {
                // To get the farmerToUpdate details from the farmDTO
                var updatedFarmer = farmDTO.Farmers.FirstOrDefault(f => f.Email == farmer.Email);
                if (updatedFarmer != null)
                {
                    // Update simple properties
                    if (!string.IsNullOrEmpty(updatedFarmer.Name)) farmer.Name = updatedFarmer.Name;
                    if (!string.IsNullOrEmpty(updatedFarmer.Location)) farmer.Location = updatedFarmer.Location;

                    farmerRepository.MarkAsModified(farmer);
                }
            }

            await farmRepository.UpdateAsync(farm);
            // Save field changes to the database
            //await fieldRepository.SaveAsync();
            // Save farmer changes to the database
            await farmerRepository.SaveAsync();

            return new FarmDTO
            {
                Id = farm.Id,
                Name = farm.Name,
                Location = farm.Location,
                Farmers = farm.Farmers.Select(farmer => new FarmerDTO
                {
                    Id = farmer.Id,
                    Name = farmer.Name,
                    Email = farmer.Email,
                    Location = farmer.Location
                }).ToList(),
                Fields = farm.Fields.Select(field => new FieldDTO
                {
                    Id = field.Id,
                    Name = field.Name,
                    CropType = field.CropType,
                    Area = field.Area
                }).ToList()
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var farm = await farmRepository.GetByIdAsync(id);
            if (farm == null) return false;
            await farmRepository.DeleteAsync(id);
            return true;
        }



    }
}

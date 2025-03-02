using FarmServer.Domain.Entities;
using FarmServer.DTOs;
using FarmServer.DTOs.Farm;
using FarmServer.Infrastructure.Repositories;
using FarmServer.Interfaces;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FarmServer.Infrastructure.Services
{
    public class FarmService : IFarmService
    {
        private readonly IFarmRepository farmRepository;

        public FarmService(IFarmRepository farmRepository) 
        {
            this.farmRepository = farmRepository;
        }

        public async Task<IEnumerable<FarmDTO>> GetAllAsync()
        {
            var farms = await farmRepository.GetAllAsync();
            // Convert farms to JSON for better logging
            Console.WriteLine(JsonSerializer.Serialize(farms, new JsonSerializerOptions { WriteIndented = true }));

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
                Location = farmDTO.Location,
                Farmers = farmDTO.Farmers.Select(farmerDTO => new Farmer
                {
                    Id = Guid.NewGuid(),
                    Name = farmerDTO.Name,
                    Email = farmerDTO.Email,
                    Location = farmerDTO.Location
                }).ToList(),
                Fields = farmDTO.Fields.Select(fieldDTO => new Field
                {
                    Id = Guid.NewGuid(),
                    Name = fieldDTO.Name,
                    CropType = fieldDTO.CropType,
                    Area = fieldDTO.Area,
                    FarmId = id
                }).ToList()
            };

            await farmRepository.AddAsync(farm);
            Console.WriteLine($"1: {farm}");

            Console.WriteLine($"2: {farmDTO}");
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



        // Update Farm
        public async Task<FarmDTO?> UpdateAsync(Guid id, CreateFarmDTO farmDTO)
        {
            var farm = await farmRepository.GetByIdAsync(id);
            Console.WriteLine($"1: {farm}");

            if (farm == null) return null;

            farm.Name = farmDTO.Name;
            farm.Location = farmDTO.Location;

            // Update Fields(Add, Modify, Remove)
            var existingFieldIds = farm.Fields.Select(f => f.Id).ToList();
            var updatedFieldIds = farmDTO.Fields.Select(f => f.Id).ToList();

            // Add new fields
            var newFields = farmDTO.Fields
                .Where(f => !existingFieldIds.Contains(f.Id))
                .Select(f => new Field
                {
                    Id = f.Id,
                    Name = f.Name,
                    CropType = f.CropType,
                    Area = f.Area,
                    FarmId = farm.Id
                }).ToList();

            farm.Fields.ToList().AddRange(newFields);
            //foreach (var newField in newFields)
            //{
            //    farm.Fields.Add(newField);
            //}

            // Update existing fields
            foreach (var field in farm.Fields)
            {
                var updatedField = farmDTO.Fields.FirstOrDefault(f => f.Id == field.Id);
                if (updatedField != null)
                {
                    field.CropType = updatedField.CropType;
                    field.Area = updatedField.Area;
                    field.Name = updatedField.Name;
                }
            }

            // Remove fields that are not in the updated DTO
            farm.Fields.ToList().RemoveAll(f => !updatedFieldIds.Contains(f.Id));

            await farmRepository.UpdateAsync(farm);
            Console.WriteLine($"2: {farm}");
            Console.WriteLine($"3: {farmDTO}");

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
            Console.WriteLine($"1: {farm}");
            if (farm == null) return false;
            await farmRepository.DeleteAsync(id);
            return true;
        }



    }
}

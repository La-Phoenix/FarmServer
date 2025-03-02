using FarmServer.Domain.Entities;

namespace FarmServer.DTOs.Farm
{
    public class FarmDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        public ICollection<FieldDTO> Fields { get; set; } = new List<FieldDTO>();
        public ICollection<FarmerDTO> Farmers { get; set; } = new List<FarmerDTO>();
    }

}

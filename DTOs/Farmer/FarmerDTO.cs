using FarmServer.DTOs.Farm;

namespace FarmServer.DTOs.Farmer
{
    public class FarmerDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Location { get; set; }

        // Many-to-Many: A Farmer can belong to multiple Farms
        public ICollection<FarmDTO> Farms { get; set; } = new List<FarmDTO>();
    }
}

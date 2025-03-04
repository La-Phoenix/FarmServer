using FarmServer.DTOs.Farm;

namespace FarmServer.DTOs.Farmer
{
    public class UpdateFarmerDTO
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Location { get; set; }

        // Many-to-Many: A Farmer can belong to multiple Farms
        public ICollection<FarmDTO> Farms { get; set; } = new List<FarmDTO>();
    }
}

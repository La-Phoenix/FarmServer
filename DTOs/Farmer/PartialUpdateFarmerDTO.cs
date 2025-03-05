using FarmServer.DTOs.Farm;

namespace FarmServer.DTOs.Farmer
{
    public class PartialUpdateFarmerDTO
    {
        public string? Name { get; set; }
        public string? Location { get; set; }

        // Many-to-Many: A Farmer can belong to multiple Farms
        public ICollection<PartialFarmUpdateDTO> Farms { get; set; } = new List<PartialFarmUpdateDTO>();
    }
}

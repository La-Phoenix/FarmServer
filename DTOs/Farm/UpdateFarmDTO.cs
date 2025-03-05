using FarmServer.DTOs.Farmer;

namespace FarmServer.DTOs.Farm
{
    public class UpdateFarmDTO
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public ICollection<FieldDTO> Fields { get; set; } = new List<FieldDTO>();
        public ICollection<CreateFarmerDTO> Farmers { get; set; } = new List<CreateFarmerDTO>();
    }
}

using FarmServer.DTOs.Farm;
using System.Text.Json.Serialization;

namespace FarmServer.DTOs.Farmer
{
    public class FarmerDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }

        [JsonIgnore]  // This prevents the password from being returned in API responses
        public required string Password { get; init; } //init allows Password to be set only during object initialization

        public required string Location { get; set; }

        // Many-to-Many: A Farmer can belong to multiple Farms
        public ICollection<FarmDTO> Farms { get; set; } = new List<FarmDTO>();
    }
}

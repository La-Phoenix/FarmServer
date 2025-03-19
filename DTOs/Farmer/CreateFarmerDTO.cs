using System.Text.Json.Serialization;

namespace FarmServer.DTOs.Farmer
{
    public class CreateFarmerDTO
    {
        public required string Name { get; set; }
        public required string Email { get; set; }

        [JsonIgnore]  // This prevents the password from being returned in API responses
        public string? Password { get; private set; } //The property can only be modified (set) within the class that defines it.

        public required string Location { get; set; }
    }
}

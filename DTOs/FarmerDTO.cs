using FarmServer.Domain.Entities;

namespace FarmServer.DTOs
{
    public class FarmerDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Location { get; set; }
    }
}

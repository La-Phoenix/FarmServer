using FarmServer.DTOs.Farm;

namespace FarmServer.DTOs.Field
{
    public class UpdateFieldDTO
    {
        public string? Name { get; set; }
        public string? CropType { get; set; }
        public decimal? Area { get; set; }

        // Foreign Key to Farm (One-to-Many Relationship)
        public Guid? FarmId { get; set; }
    }
}


using FarmServer.DTOs.Farm;

namespace FarmServer.DTOs.Field
{
    public class FieldDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string CropType { get; set; }
        public required decimal Area { get; set; }

        // Foreign Key to Farm (One-to-Many Relationship)
        public required Guid FarmId { get; set; }
        public FarmDTO Farm { get; set; } = null!;
    }
}

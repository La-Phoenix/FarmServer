namespace FarmServer.DTOs.Field
{
    public class CreateFieldDTO
    {
        public required string Name { get; set; }
        public required string CropType { get; set; }
        public required decimal Area { get; set; }

        // Foreign Key to Farm (One-to-Many Relationship)
        public required Guid FarmId { get; set; }
    }
}

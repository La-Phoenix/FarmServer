namespace FarmServer.Domain.Entities
{
    public class Field
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string CropType { get; set; }
        public required decimal Area { get; set; }

        // Foreign Key to Farm (One-to-Many Relationship)
        public required Guid FarmId { get; set; }
        public Farm Farm { get; set; }  = null!;
    }
}

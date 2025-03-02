using FarmServer.Domain.Entities;

namespace FarmServer.DTOs
{
    public class FieldDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string CropType { get; set; }
        public required decimal Area { get; set; }
    }
}

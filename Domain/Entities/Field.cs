using FarmServer.DTOs.Farm;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        // Navigation property to include Farm details
        [ForeignKey("FarmId")]
        public Farm Farm { get; set; } = null!;
    }
}

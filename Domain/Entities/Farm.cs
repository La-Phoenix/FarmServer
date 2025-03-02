namespace FarmServer.Domain.Entities
{
    public class Farm
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }

        // One-to-Many: A Farm has many Fields
        public ICollection<Field> Fields { get; set; } = new List<Field>();

        // Many-to-Many: A Farm has many Farmers, and a Farmer can belong to multiple Farms
        public ICollection<Farmer> Farmers { get; set; } = new List<Farmer>();
    }
}

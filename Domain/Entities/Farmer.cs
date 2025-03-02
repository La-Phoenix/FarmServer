namespace FarmServer.Domain.Entities
{
    public class Farmer
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Location { get; set; }

        // Many-to-Many: A Farmer can belong to multiple Farms
        public ICollection<Farm> Farms { get; set; } = new List<Farm>();
    }
}

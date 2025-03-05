namespace FarmServer.DTOs.Farm
{
    public class PartialFarmUpdateDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
    }
}

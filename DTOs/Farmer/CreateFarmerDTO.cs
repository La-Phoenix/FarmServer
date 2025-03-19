namespace FarmServer.DTOs.Farmer
{
    public class CreateFarmerDTO
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        public required string Location { get; set; }
    }
}

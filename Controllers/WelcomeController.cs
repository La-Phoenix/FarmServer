using FarmServer.DTOs.Farmer;
using FarmServer.Infrastructure.Services;
using FarmServer.Interfaces.IFarmer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FarmServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class WelcomeController : ControllerBase
    {
        private readonly IFarmerService farmerService;
        private readonly ILogger<WelcomeController> logger;
        private readonly JwtService jwtService;

        public WelcomeController(IFarmerService farmerService, ILogger<WelcomeController> logger, JwtService jwtService)
        {
            this.farmerService = farmerService;
            this.logger = logger;
            this.jwtService = jwtService;
        }

        [HttpGet]
        public IActionResult GetWelcomeMessage()
        {
            return Ok(new { message = "Welcome to LaPhoenix's FarmServer! 🚜🌱" });
        }

         [HttpPost("login")]
        public async Task<ActionResult<FarmerDTO>> Login([FromBody] FarmerLoginDTO farmerLoginDTO)
        {
            try
            {
                //if (farmerLoginDTO == null || string.IsNullOrWhiteSpace(farmerLoginDTO.Email) || string.IsNullOrWhiteSpace(farmerLoginDTO.Password)) ;
                if (farmerLoginDTO == null || string.IsNullOrWhiteSpace(farmerLoginDTO.Email)) return BadRequest(new { message = "Invalid login credentials." });
                var farmer = await farmerService.Login(farmerLoginDTO);
                if (farmer == null) return Unauthorized(new { message = "Invalid email or password." });
                var token = jwtService.GenerateToken(farmer.Id, farmer.Email);

                return Ok(new
                {
                    farmer,
                    token
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loging in");
                return Problem(detail: "An error occurred while validating farmer.", statusCode: 500);
            }
        }
    }
}

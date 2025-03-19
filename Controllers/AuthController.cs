using FarmServer.DTOs.Farmer;
using FarmServer.Infrastructure.Services;
using FarmServer.Interfaces.IFarmer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FarmServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IFarmerService farmerService;
        private readonly JwtService jwtService;
        private readonly ILogger<AuthController> logger;

        public AuthController(IFarmerService farmerService, JwtService jwtService, ILogger<AuthController> logger)
        {
            this.farmerService = farmerService;
            this.jwtService = jwtService;
            this.logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<FarmerDTO>> Login([FromBody] FarmerLoginDTO farmerLoginDTO)
        {
            try
            {
                if (farmerLoginDTO == null || string.IsNullOrWhiteSpace(farmerLoginDTO.Email) || string.IsNullOrWhiteSpace(farmerLoginDTO.Password)) return BadRequest(new { message = "Invalid login credentials." });
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
                logger.LogError(ex, $"Error logging in user with email {farmerLoginDTO.Email}");
                return StatusCode(500, new { message = "An error occurred while validating farmer." });
            }
        }
    }
}

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
        public ContentResult Welcome()
        {
            string html = @"
                <html>
                <head><title>FarmServer API</title></head>
                <body>
                    <h1>Welcome to FarmServer API 🌱</h1>
                    <p>This API helps manage farmers, farms, and produce.</p>
                    <p>📌 <strong>Base URL:</strong> <code>https://farmserver.onrender.com/api</code></p>
                    <p>🔑 <strong>Authentication:</strong> Use Bearer Token from <code>/api/auth/login</code></p>
                    <p>📜 <strong>API Docs & Testing:</strong> <a href='https://red-star-5463.postman.co/workspace/Personal~0dd7abf0-ba43-4be5-b08b-f4ccc6d6bbeb/collection/40959837-e67c1cea-03a7-4dc4-99eb-1c5c144914cb?action=share&creator=40959837' target='_blank'>Open in Postman</a></p>
                </body>
                </html>";
            return new ContentResult { Content = html, ContentType = "text/html" };
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

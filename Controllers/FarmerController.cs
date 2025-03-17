using FarmServer.DTOs.Farmer;
using FarmServer.Infrastructure.Services;
using FarmServer.Interfaces.IFarmer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FarmerController : ControllerBase
    {
        private readonly IFarmerService farmerService;
        private readonly ILogger<FarmerController> logger;
        private readonly JwtService jwtService;

        public FarmerController(IFarmerService farmerService, ILogger<FarmerController> logger, JwtService jwtService)
        {
            this.farmerService = farmerService;
            this.logger = logger;
            this.jwtService = jwtService;
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

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FarmerDTO>>> GetAll()
        {
            try
            {
                var farmers = await farmerService.GetAllAsync();
                return Ok(farmers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving farmers");
                return Problem(detail: "An error occurred while fetching farmers.", statusCode: 500);
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<FarmerDTO>> GetById(Guid id)
        {
            try
            {
                var farmer = await farmerService.GetByIdAsync(id);
                if (farmer == null) return NotFound(new { message = $"Farmer with id: {id}, NOT FOUND." });
                return Ok(farmer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving farmers");
                return Problem(detail: $"An error occurred while fetching farmer: {id}.", statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<FarmerDTO>> Create([FromBody] CreateFarmerDTO farmerDto)
        {
            if (farmerDto == null) return BadRequest(new { message = "Invalid farmer data provided." });
            try
            {
                var farmerExist = farmerDto.Email != null ? await farmerService.GetByEmailAsync(farmerDto.Email) : null;
                if (farmerExist != null) return BadRequest(new { message = $"Farmer with email: {farmerDto.Email}, already exists." });
                var farmer = await farmerService.CreateAsync(farmerDto);
                if (farmer == null) return Problem(detail: "An error occurred while creating farmer.", statusCode: 500);
                //Generate JWT
                var token = jwtService.GenerateToken(farmer.Id, farmer.Email);
                return CreatedAtAction(nameof(GetById), new { id = farmer.Id }, new
                {
                    farmer,
                    token,
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating farmer");
                return Problem(detail: "An error occurred while creating farmer.", statusCode: 500);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<FarmerDTO>> Update(Guid id, [FromBody] PartialUpdateFarmerDTO farmerDto)
        {
            if (farmerDto == null) return BadRequest(new { message = "Invalid farmer data provided." });
            try
            {
                var farmer = await farmerService.UpdateAsync(id, farmerDto);
                if (farmer == null) return NotFound(new { message = $"Farmer with id: {id}, NOT FOUND." });
                return Ok(farmer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating farmer");
                return Problem(detail: $"An error occurred while updating farmer: {id}.", statusCode: 500);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var result = await farmerService.DeleteAsync(id);
                if (!result) return NotFound(new { message = $"Farmer with id: {id}, NOT FOUND." });
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting farmer");
                return Problem(detail: $"An error occurred while deleting farmer: {id}.", statusCode: 500);
            }
        }
    }
}

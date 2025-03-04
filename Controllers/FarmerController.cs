using FarmServer.DTOs.Farmer;
using FarmServer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FarmServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FarmerController : ControllerBase
    {
        private readonly IFarmerService farmerService;
        private readonly ILogger<FarmerController> logger;

        public FarmerController(IFarmerService farmerService, ILogger<FarmerController> logger)
        {
            this.farmerService = farmerService;
            this.logger = logger;
        }


        [HttpGet]
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
                var farmer = await farmerService.CreateAsync(farmerDto);
                return CreatedAtAction(nameof(GetById), new { id = farmer.Id }, farmer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating farmer");
                return Problem(detail: "An error occurred while creating farmer.", statusCode: 500);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FarmerDTO>> Update(Guid id, [FromBody] UpdateFarmerDTO farmerDto)
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

using FarmServer.DTOs.Farm;
using FarmServer.Infrastructure.Services;
using FarmServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FarmController : Controller
    {
        private readonly IFarmService farmService;
        private readonly ILogger<FarmController> logger;

        public FarmController(IFarmService farmService, ILogger<FarmController> logger)
        {
            this.farmService = farmService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FarmDTO>>> GetAll()
        {
            try
            {
                var farms = await farmService.GetAllAsync();
                return Ok(farms);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving farms");
                return Problem(detail: "An error occurred while fetching farms.", statusCode: 500);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FarmDTO>> GetById(Guid id)
        {
            try
            {
                var farm = await farmService.GetByIdAsync(id);
                if (farm == null) return NotFound(new { message = $"Farm with id: {id}, NOT FOUND."});
                return Ok(farm);
            } catch(Exception ex)
            {
                logger.LogError(ex, "Error retrieving farms");
                return Problem(detail: $"An error occurred while fetching farm: {id}.", statusCode: 500);
            }
            
        }

        [HttpPost]
        public async Task<ActionResult<FarmDTO>> Create([FromBody] CreateFarmDTO farmDto)
        {
            if (farmDto == null) return BadRequest(new { message = "Invalid farm data provided." });
            try
            {
                var farm = await farmService.CreateAsync(farmDto);
                return CreatedAtAction(nameof(GetById), new { id = farm.Id }, farm);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Creating farm ");
                return Problem(detail: $"An error occurred while creating farm.", statusCode: 500);
            }
            
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FarmDTO>> Update(Guid id, [FromBody] CreateFarmDTO farmDto)
        {
            if (farmDto == null) return BadRequest(new { message = "Invalid Farm Data Provided." });
            try
            {
                var updatedFarm = await farmService.UpdateAsync(id, farmDto);
                if (updatedFarm == null) return NotFound(new { message = $"Farm with id: {id}, not found for update." });
                return Ok(updatedFarm);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error deleting farm with id: {id}");
                return Problem(detail: $"An error occurred while deleting farm: {id}.", statusCode: 500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var result = await farmService.DeleteAsync(id);
                if (!result) return NotFound(new {message = $"Farm with id: {id}, not found for deletion." });
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error deleting farm with id: {id}");
                return Problem(detail: $"An error occurred while deleting farm: {id}.", statusCode: 500);
            }
        }
    }
}

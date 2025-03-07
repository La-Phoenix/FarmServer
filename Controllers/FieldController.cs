using FarmServer.DTOs.Field;
using FarmServer.Interfaces.IFarm;
using FarmServer.Interfaces.IField;
using Microsoft.AspNetCore.Mvc;


namespace FarmServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly IFieldService fieldService;
        private readonly ILogger<FieldController> logger;
        private readonly IFarmService farmService;

        public FieldController(IFieldService fieldService, ILogger<FieldController> logger, IFarmService farmService)
        {
            this.fieldService = fieldService;
            this.logger = logger;
            this.farmService = farmService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FieldDTO>>> GetAll()
        {
            try
            {
                var fields = await fieldService.GetAllAsync();
                return Ok(fields);
            }
            catch (Exception ex) 
            {
                logger.LogError(ex, "Error retrieving fields");
                return Problem(detail: "An error occurred while fetching fields.", statusCode: 500);
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<FieldDTO>> GetById(Guid id)
        {
            try
            {
                var field = await fieldService.GetByIdAsync(id);
                if (field == null) return NotFound(new { message = $"Field with id: {id}, NOT FOUND." });
                return Ok(field);
            } catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving fields");
                return Problem(detail: $"An error occurred while fetching field: {id}", statusCode: 500);
            }

        }

        [HttpPost]
        public async Task<ActionResult<FieldDTO>> CreateAsync([FromBody] CreateFieldDTO createFieldDTO)
        {
            try
            {
                if (createFieldDTO == null) return BadRequest(new { message = "Invalid field data provided." });
                var field = await fieldService.CreateAsync(createFieldDTO);
                if (field == null) return Problem(detail: $"Farm with id: {createFieldDTO.FarmId}, NOT FOUND.", statusCode: 404);
                if (field.Id == Guid.Empty)
                    return Problem(detail: "Field creation failed.", statusCode: 500);

                return CreatedAtAction(nameof(GetById), new { id = field.Id }, field);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating field");
                return Problem(detail: "An error occurred while creating field", statusCode: 500);
            }
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<FieldDTO>> UpdateAsync(Guid id, [FromBody] UpdateFieldDTO fieldDTO)
        {
            try
            {
                if (fieldDTO == null) return BadRequest(new { message = "Invalid field data provided." });
                if (fieldDTO.FarmId.HasValue)
                {
                    var farmExist = await farmService.GetByIdAsync(fieldDTO.FarmId.Value);
                    if (farmExist == null) return Problem(detail: $"Farm with id: {fieldDTO.FarmId.Value}, NOT FOUND.", statusCode: 404);
                }
                var updatedField = await fieldService.UpdateAsync(id, fieldDTO);
                if (updatedField == null) return NotFound(new { message = $"Field with id: {id}, NOT FOUND." });
                return Ok(updatedField);
            }
            catch (Exception ex)
            {
                 logger.LogError(ex, "Error updating field");
                return Problem(detail: $"An error occurred while updating field: {id}", statusCode: 500);
            }
        }

       
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var isDeleted = await fieldService.DeleteAsync(id);
                if (!isDeleted) return NotFound(new { message = $"Field with id: {id}, NOT FOUND." });
                return NoContent();
            } catch(Exception ex)
            {
                logger.LogError(ex, "Error deleting field");
                return Problem(detail: $"An error occurred while deleting field: {id}", statusCode: 500);
            }
        }
    }
}

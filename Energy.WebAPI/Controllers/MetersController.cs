using Energy.WebAPI.DTOs.MeterDTOs;
using Energy.WebAPI.Repositories.Meters;
using Microsoft.AspNetCore.Mvc;

namespace Energy.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetersController : ControllerBase
    {
        private readonly IMeterService _meterService;

        public MetersController(IMeterService meterService)
        {
            _meterService = meterService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ResultMeterDTO>>> GetList()
        {
            return Ok(await _meterService.GetListAsync());
        }

        [HttpGet("overview")]
        public async Task<ActionResult<MeterOverviewDto>> GetOverview()
        {
            return Ok(await _meterService.GetOverviewAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResultMeterDTO>> GetById(int id)
        {
            var row = await _meterService.GetByIdAsync(id);
            if (row == null)
                return NotFound();
            return Ok(row);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMeterDTO dto)
        {
            await _meterService.CreateAsync(dto);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMeterDTO dto)
        {
            dto.Id = id;
            await _meterService.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _meterService.DeleteAsync(id);
            return NoContent();
        }
    }
}

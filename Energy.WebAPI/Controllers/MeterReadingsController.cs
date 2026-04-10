using Energy.WebAPI.DTOs.MeterReadingDTOs;
using Energy.WebAPI.Repositories.MeterReadings;
using Microsoft.AspNetCore.Mvc;

namespace Energy.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeterReadingsController : ControllerBase
    {
        private readonly IMeterReadingService _meterReadingService;

        public MeterReadingsController(IMeterReadingService meterReadingService)
        {
            _meterReadingService = meterReadingService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ResultMeterReadingDTO>>> GetList()
        {
            return Ok(await _meterReadingService.GetListAsync());
        }

        [HttpGet("with-region")]
        public async Task<ActionResult<List<ResultMeterReadingWithRegionDTO>>> GetListWithRegion()
        {
            return Ok(await _meterReadingService.GetListWithRegionAsync());
        }

        [HttpGet("overview")]
        public async Task<ActionResult<MeterReadingOverviewDto>> GetOverview()
        {
            return Ok(await _meterReadingService.GetOverviewAsync());
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ResultMeterReadingDTO>> GetById(long id)
        {
            var row = await _meterReadingService.GetByIdAsync(id);
            if (row == null)
                return NotFound();
            return Ok(row);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMeterReadingDTO dto)
        {
            await _meterReadingService.CreateAsync(dto);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateMeterReadingDTO dto)
        {
            dto.Id = id;
            await _meterReadingService.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _meterReadingService.DeleteAsync(id);
            return NoContent();
        }
    }
}

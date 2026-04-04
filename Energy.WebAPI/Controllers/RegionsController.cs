using Energy.WebAPI.DTOs.RegionDTOs;
using Energy.WebAPI.Repositories.Regions;
using Microsoft.AspNetCore.Mvc;

namespace Energy.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;

        public RegionsController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ResultRegionDTO>>> GetList()
        {
            return Ok(await _regionService.GetListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResultRegionDTO>> GetById(int id)
        {
            var row = await _regionService.GetByIdAsync(id);
            if (row == null)
                return NotFound();
            return Ok(row);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRegionDTO dto)
        {
            await _regionService.CreateAsync(dto);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRegionDTO dto)
        {
            dto.Id = id;
            await _regionService.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _regionService.DeleteAsync(id);
            return NoContent();
        }
    }
}

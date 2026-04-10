using Energy.WebAPI.DTOs.RegionDTOs;

namespace Energy.WebAPI.Repositories.Regions
{
    public interface IRegionService
    {
        Task<List<ResultRegionDTO>> GetListAsync();
        Task<RegionOverviewDto> GetOverviewAsync();
        Task CreateAsync(CreateRegionDTO createRegionDTO);
        Task UpdateAsync(UpdateRegionDTO updateRegionDTO);
        Task DeleteAsync(int id);
        Task<ResultRegionDTO> GetByIdAsync(int id);
    }
}

using Energy.WebAPI.DTOs.MeterDTOs;

namespace Energy.WebAPI.Repositories.Meters
{
    public interface IMeterService
    {
        Task<List<ResultMeterDTO>> GetListAsync();
        Task<MeterOverviewDto> GetOverviewAsync();
        Task CreateAsync(CreateMeterDTO createMeterDTO);
        Task UpdateAsync(UpdateMeterDTO updateMeterDTO);
        Task DeleteAsync(int id);
        Task<ResultMeterDTO> GetByIdAsync(int id);
    }
}

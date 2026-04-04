using Energy.WebAPI.DTOs.MeterReadingDTOs;

namespace Energy.WebAPI.Repositories.MeterReadings
{
    public interface IMeterReadingService
    {
        Task<List<ResultMeterReadingDTO>> GetListAsync();
        Task CreateAsync(CreateMeterReadingDTO createMeterReadingDTO);
        Task UpdateAsync(UpdateMeterReadingDTO updateMeterReadingDTO);
        Task DeleteAsync(long id);
        Task<ResultMeterReadingDTO> GetByIdAsync(long id);
    }
}

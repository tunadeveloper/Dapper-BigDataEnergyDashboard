using Energy.WebAPI.DTOs.DashboardDTOs;

namespace Energy.WebAPI.Repositories.Dashboard
{
    public interface IDashboardRepository
    {
        Task<EnergyDashboardDto> GetDashboardStatsAsync(DateTime start, DateTime end);
    }
}

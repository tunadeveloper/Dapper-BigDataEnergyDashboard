using Dapper;
using Energy.WebAPI.Context;
using Energy.WebAPI.DTOs.DashboardDTOs;
using Microsoft.Data.SqlClient;

namespace Energy.WebAPI.Repositories.Dashboard
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DapperContext _context;

        public DashboardRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<EnergyDashboardDto> GetDashboardStatsAsync(DateTime start, DateTime end)
        {
            const string sql = "SELECT (SELECT ISNULL(SUM(Consumption), 0) FROM MeterReadings WHERE ReadingDate >= @Start AND ReadingDate <= @End) AS TotalConsumption, (SELECT COUNT(*) FROM MeterReadings WHERE ReadingDate >= @Start AND ReadingDate <= @End) AS ReadingCount, (SELECT COUNT(*) FROM Meters WHERE IsActive = 1) AS ActiveMeterCount, (SELECT COUNT(*) FROM Regions) AS RegionCount";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            return await connection.QueryFirstAsync<EnergyDashboardDto>(sql, new { Start = start, End = end });
        }
    }
}

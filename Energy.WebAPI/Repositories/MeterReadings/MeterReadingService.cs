using Dapper;
using Energy.WebAPI.Context;
using Energy.WebAPI.DTOs.MeterReadingDTOs;
using Energy.WebAPI.Redis;
using Microsoft.Data.SqlClient;

namespace Energy.WebAPI.Repositories.MeterReadings
{
    public class MeterReadingService : IMeterReadingService
    {
        private const string MeterReadingListKey = "meterreadings:list";
        private const string MeterReadingListWithRegionKey = "meterreadings:list:region";
        private const string MeterReadingOverviewKey = "meterreadings:overview";
        private const string MeterOverviewKey = "meters:overview";
        private const string RegionOverviewKey = "regions:overview";
        private static string MeterReadingByIdKey(long id) => $"meterreadings:{id}";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(15);

        private readonly DapperContext _context;
        private readonly IRedisCacheService _cache;

        public MeterReadingService(DapperContext context, IRedisCacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task CreateAsync(CreateMeterReadingDTO createMeterReadingDTO)
        {
            const string sql = "INSERT INTO MeterReadings (MeterId, Consumption, Voltage, ReadingDate, TariffType) VALUES (@MeterId, @Consumption, @Voltage, @ReadingDate, @TariffType)";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, createMeterReadingDTO);
            _cache.RemoveData(MeterReadingListKey);
            _cache.RemoveData(MeterReadingListWithRegionKey);
            _cache.RemoveData(MeterReadingOverviewKey);
            _cache.RemoveData(MeterOverviewKey);
            _cache.RemoveData(RegionOverviewKey);
        }

        public async Task DeleteAsync(long id)
        {
            const string sql = "DELETE FROM MeterReadings WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, new { Id = id });
            _cache.RemoveData(MeterReadingListKey);
            _cache.RemoveData(MeterReadingListWithRegionKey);
            _cache.RemoveData(MeterReadingOverviewKey);
            _cache.RemoveData(MeterOverviewKey);
            _cache.RemoveData(RegionOverviewKey);
            _cache.RemoveData(MeterReadingByIdKey(id));
        }

        public async Task<ResultMeterReadingDTO> GetByIdAsync(long id)
        {
            var cached = _cache.GetData<ResultMeterReadingDTO>(MeterReadingByIdKey(id));
            if (cached != null)
                return cached;

            const string sql = "SELECT Id, MeterId, Consumption, Voltage, ReadingDate, TariffType FROM MeterReadings WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            var row = await connection.QueryFirstOrDefaultAsync<ResultMeterReadingDTO>(sql, new { Id = id });
            if (row != null)
                _cache.SetData(MeterReadingByIdKey(id), row, CacheTtl);
            return row!;
        }

        public async Task<List<ResultMeterReadingDTO>> GetListAsync()
        {
            var cached = _cache.GetData<List<ResultMeterReadingDTO>>(MeterReadingListKey);
            if (cached != null)
                return cached;

            const string sql = "SELECT Id, MeterId, Consumption, Voltage, ReadingDate, TariffType FROM MeterReadings ORDER BY Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            var list = (await connection.QueryAsync<ResultMeterReadingDTO>(sql)).ToList();
            _cache.SetData(MeterReadingListKey, list, CacheTtl);
            return list;
        }

        public async Task<List<ResultMeterReadingWithRegionDTO>> GetListWithRegionAsync()
        {
            var cached = _cache.GetData<List<ResultMeterReadingWithRegionDTO>>(MeterReadingListWithRegionKey);
            if (cached != null)
                return cached;

            const string sql = "SELECT mr.Id, mr.MeterId, mr.Consumption, mr.Voltage, mr.ReadingDate, mr.TariffType, reg.RegionName FROM MeterReadings mr INNER JOIN Meters m ON mr.MeterId = m.Id INNER JOIN Regions reg ON m.RegionId = reg.Id ORDER BY mr.ReadingDate DESC";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            var list = (await connection.QueryAsync<ResultMeterReadingWithRegionDTO>(sql)).ToList();
            _cache.SetData(MeterReadingListWithRegionKey, list, CacheTtl);
            return list;
        }

        public async Task<MeterReadingOverviewDto> GetOverviewAsync()
        {
            var cached = _cache.GetData<MeterReadingOverviewDto>(MeterReadingOverviewKey);
            if (cached != null)
                return cached;

            var utcNow = DateTime.UtcNow;
            var dayStart = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
            const string sql = "SELECT ISNULL(SUM(CASE WHEN ReadingDate >= @DayStart THEN 1 ELSE 0 END), 0) AS TodayReadingCount, ISNULL(SUM(Consumption), 0) AS TotalConsumption, ISNULL(AVG(CAST(Voltage AS DECIMAL(18,2))), 0) AS AverageVoltage, ISNULL(SUM(CASE WHEN Voltage < 210 OR Voltage > 240 THEN 1 ELSE 0 END), 0) AS AnomalyCount FROM MeterReadings; SELECT TOP (20) mr.Id, mr.MeterId, mr.Consumption, mr.Voltage, mr.ReadingDate, mr.TariffType, reg.RegionName FROM MeterReadings mr INNER JOIN Meters m ON mr.MeterId = m.Id INNER JOIN Regions reg ON m.RegionId = reg.Id ORDER BY mr.ReadingDate DESC;";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            using var multi = await connection.QueryMultipleAsync(sql, new { DayStart = dayStart });
            var overview = await multi.ReadFirstAsync<MeterReadingOverviewDto>();
            overview.LatestReadings = (await multi.ReadAsync<ResultMeterReadingWithRegionDTO>()).ToList();
            _cache.SetData(MeterReadingOverviewKey, overview, CacheTtl);
            return overview;
        }

        public async Task UpdateAsync(UpdateMeterReadingDTO updateMeterReadingDTO)
        {
            const string sql = "UPDATE MeterReadings SET MeterId = @MeterId, Consumption = @Consumption, Voltage = @Voltage, ReadingDate = @ReadingDate, TariffType = @TariffType WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, updateMeterReadingDTO);
            _cache.RemoveData(MeterReadingListKey);
            _cache.RemoveData(MeterReadingListWithRegionKey);
            _cache.RemoveData(MeterReadingOverviewKey);
            _cache.RemoveData(MeterOverviewKey);
            _cache.RemoveData(RegionOverviewKey);
            _cache.RemoveData(MeterReadingByIdKey(updateMeterReadingDTO.Id));
        }
    }
}

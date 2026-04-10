using Dapper;
using Energy.WebAPI.Context;
using Energy.WebAPI.DTOs.RegionDTOs;
using Energy.WebAPI.Redis;
using Microsoft.Data.SqlClient;

namespace Energy.WebAPI.Repositories.Regions
{
    public class RegionService : IRegionService
    {
        private const string RegionListKey = "regions:list";
        private const string RegionOverviewKey = "regions:overview";
        private static string RegionByIdKey(int id) => $"regions:{id}";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(15);

        private readonly DapperContext _context;
        private readonly IRedisCacheService _cache;

        public RegionService(DapperContext context, IRedisCacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task CreateAsync(CreateRegionDTO createRegionDTO)
        {
            const string sql = "INSERT INTO Regions (RegionName) VALUES (@RegionName)";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, createRegionDTO);
            _cache.RemoveData(RegionListKey);
            _cache.RemoveData(RegionOverviewKey);
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Regions WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, new { Id = id });
            _cache.RemoveData(RegionListKey);
            _cache.RemoveData(RegionOverviewKey);
            _cache.RemoveData(RegionByIdKey(id));
        }

        public async Task<ResultRegionDTO> GetByIdAsync(int id)
        {
            var cached = _cache.GetData<ResultRegionDTO>(RegionByIdKey(id));
            if (cached != null)
                return cached;

            const string sql = "SELECT Id, RegionName FROM Regions WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            var row = await connection.QueryFirstOrDefaultAsync<ResultRegionDTO>(sql, new { Id = id });
            if (row != null)
                _cache.SetData(RegionByIdKey(id), row, CacheTtl);
            return row!;
        }

        public async Task<List<ResultRegionDTO>> GetListAsync()
        {
            var cached = _cache.GetData<List<ResultRegionDTO>>(RegionListKey);
            if (cached != null)
                return cached;

            const string sql = "SELECT Id, RegionName FROM Regions ORDER BY Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            var list = (await connection.QueryAsync<ResultRegionDTO>(sql)).ToList();
            _cache.SetData(RegionListKey, list, CacheTtl);
            return list;
        }

        public async Task<RegionOverviewDto> GetOverviewAsync()
        {
            var cached = _cache.GetData<RegionOverviewDto>(RegionOverviewKey);
            if (cached != null)
                return cached;

            const string sql = "SELECT (SELECT COUNT(*) FROM Regions) AS TotalRegionCount, ISNULL((SELECT TOP 1 reg.RegionName FROM MeterReadings mr INNER JOIN Meters m ON mr.MeterId = m.Id INNER JOIN Regions reg ON m.RegionId = reg.Id GROUP BY reg.RegionName ORDER BY SUM(mr.Consumption) DESC), '-') AS TopConsumptionRegionName, ISNULL((SELECT TOP 1 SUM(mr.Consumption) FROM MeterReadings mr INNER JOIN Meters m ON mr.MeterId = m.Id INNER JOIN Regions reg ON m.RegionId = reg.Id GROUP BY reg.RegionName ORDER BY SUM(mr.Consumption) DESC), 0) AS TopConsumptionValue, ISNULL((SELECT TOP 1 reg.RegionName FROM Regions reg LEFT JOIN Meters m ON m.RegionId = reg.Id GROUP BY reg.RegionName ORDER BY COUNT(m.Id) DESC), '-') AS TopMeterRegionName, ISNULL((SELECT TOP 1 COUNT(m.Id) FROM Regions reg LEFT JOIN Meters m ON m.RegionId = reg.Id GROUP BY reg.RegionName ORDER BY COUNT(m.Id) DESC), 0) AS TopMeterRegionCount, ISNULL((SELECT AVG(CAST(mr.Voltage AS DECIMAL(18,2))) FROM MeterReadings mr), 0) AS AverageVoltage; SELECT reg.RegionName, ISNULL(SUM(mr.Consumption), 0) AS TotalConsumption, COUNT(DISTINCT m.Id) AS MeterCount, ISNULL(AVG(CAST(mr.Voltage AS DECIMAL(18,2))), 0) AS AverageVoltage FROM Regions reg LEFT JOIN Meters m ON m.RegionId = reg.Id LEFT JOIN MeterReadings mr ON mr.MeterId = m.Id GROUP BY reg.RegionName ORDER BY TotalConsumption DESC;";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            using var multi = await connection.QueryMultipleAsync(sql);
            var overview = await multi.ReadFirstAsync<RegionOverviewDto>();
            var rows = (await multi.ReadAsync<RegionOverviewItemDto>()).ToList();
            var totalConsumption = rows.Sum(x => x.TotalConsumption);
            foreach (var row in rows)
                row.SharePercent = totalConsumption == 0 ? 0 : Math.Round(row.TotalConsumption * 100 / totalConsumption, 1);
            overview.Regions = rows;
            _cache.SetData(RegionOverviewKey, overview, CacheTtl);
            return overview;
        }

        public async Task UpdateAsync(UpdateRegionDTO updateRegionDTO)
        {
            const string sql = "UPDATE Regions SET RegionName = @RegionName WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, updateRegionDTO);
            _cache.RemoveData(RegionListKey);
            _cache.RemoveData(RegionOverviewKey);
            _cache.RemoveData(RegionByIdKey(updateRegionDTO.Id));
        }
    }
}

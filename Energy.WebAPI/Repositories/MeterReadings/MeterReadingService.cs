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
        }

        public async Task DeleteAsync(long id)
        {
            const string sql = "DELETE FROM MeterReadings WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, new { Id = id });
            _cache.RemoveData(MeterReadingListKey);
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

        public async Task UpdateAsync(UpdateMeterReadingDTO updateMeterReadingDTO)
        {
            const string sql = "UPDATE MeterReadings SET MeterId = @MeterId, Consumption = @Consumption, Voltage = @Voltage, ReadingDate = @ReadingDate, TariffType = @TariffType WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, updateMeterReadingDTO);
            _cache.RemoveData(MeterReadingListKey);
            _cache.RemoveData(MeterReadingByIdKey(updateMeterReadingDTO.Id));
        }
    }
}

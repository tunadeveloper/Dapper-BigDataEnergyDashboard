using Dapper;
using Energy.WebAPI.Context;
using Energy.WebAPI.DTOs.MeterDTOs;
using Energy.WebAPI.Redis;
using Microsoft.Data.SqlClient;

namespace Energy.WebAPI.Repositories.Meters
{
    public class MeterService : IMeterService
    {
        private const string MeterListKey = "meters:list";
        private static string MeterByIdKey(int id) => $"meters:{id}";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(15);

        private readonly DapperContext _context;
        private readonly IRedisCacheService _cache;

        public MeterService(DapperContext context, IRedisCacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task CreateAsync(CreateMeterDTO createMeterDTO)
        {
            const string sql = "INSERT INTO Meters (SerialNumber, RegionId, SubscriptionDate, IsActive) VALUES (@SerialNumber, @RegionId, @SubscriptionDate, @IsActive)";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, createMeterDTO);
            _cache.RemoveData(MeterListKey);
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Meters WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, new { Id = id });
            _cache.RemoveData(MeterListKey);
            _cache.RemoveData(MeterByIdKey(id));
        }

        public async Task<ResultMeterDTO> GetByIdAsync(int id)
        {
            var cached = _cache.GetData<ResultMeterDTO>(MeterByIdKey(id));
            if (cached != null)
                return cached;

            const string sql = "SELECT Id, SerialNumber, RegionId, SubscriptionDate, IsActive FROM Meters WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            var row = await connection.QueryFirstOrDefaultAsync<ResultMeterDTO>(sql, new { Id = id });
            if (row != null)
                _cache.SetData(MeterByIdKey(id), row, CacheTtl);
            return row!;
        }

        public async Task<List<ResultMeterDTO>> GetListAsync()
        {
            var cached = _cache.GetData<List<ResultMeterDTO>>(MeterListKey);
            if (cached != null)
                return cached;

            const string sql = "SELECT Id, SerialNumber, RegionId, SubscriptionDate, IsActive FROM Meters ORDER BY Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            var list = (await connection.QueryAsync<ResultMeterDTO>(sql)).ToList();
            _cache.SetData(MeterListKey, list, CacheTtl);
            return list;
        }

        public async Task UpdateAsync(UpdateMeterDTO updateMeterDTO)
        {
            const string sql = "UPDATE Meters SET SerialNumber = @SerialNumber, RegionId = @RegionId, SubscriptionDate = @SubscriptionDate, IsActive = @IsActive WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, updateMeterDTO);
            _cache.RemoveData(MeterListKey);
            _cache.RemoveData(MeterByIdKey(updateMeterDTO.Id));
        }
    }
}

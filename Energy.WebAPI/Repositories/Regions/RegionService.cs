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
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Regions WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, new { Id = id });
            _cache.RemoveData(RegionListKey);
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

        public async Task UpdateAsync(UpdateRegionDTO updateRegionDTO)
        {
            const string sql = "UPDATE Regions SET RegionName = @RegionName WHERE Id = @Id";
            await using var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, updateRegionDTO);
            _cache.RemoveData(RegionListKey);
            _cache.RemoveData(RegionByIdKey(updateRegionDTO.Id));
        }
    }
}

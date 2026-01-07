using MongoDB.Bson;

namespace TrackerHabiHamApi.Services
{
    public interface IJsonStoreService
    {
        Task<string> CreateAsync(BsonDocument payload, string? id = null, CancellationToken ct = default);
        Task<BsonDocument?> GetAsync(string id, CancellationToken ct = default);
        Task<bool> CheckConnectionAsync(CancellationToken ct = default);
    }
}


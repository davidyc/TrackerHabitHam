using MongoDB.Bson;
using MongoDB.Driver;
using TrackerHabiHamApi.Models;

namespace TrackerHabiHamApi.Services
{
    public class JsonStoreService : IJsonStoreService
    {
        private readonly IMongoCollection<JsonItem> _collection;
        private readonly IMongoDatabase _database;

        public JsonStoreService(IMongoDatabase database)
        {
            _database = database;
            _collection = database.GetCollection<JsonItem>("json_items");
            
            // Create index on CreatedAt (descending)
            var indexKeys = Builders<JsonItem>.IndexKeys.Descending(x => x.CreatedAt);
            var indexOptions = new CreateIndexOptions { Background = true };
            _collection.Indexes.CreateOne(
                new CreateIndexModel<JsonItem>(indexKeys, indexOptions));
        }

        public async Task<string> CreateAsync(BsonDocument payload, string? id = null, CancellationToken ct = default)
        {
            // Generate ID if not provided
            if (string.IsNullOrWhiteSpace(id))
            {
                id = ObjectId.GenerateNewId().ToString();
            }

            // Check if item already exists to preserve CreatedAt
            var filter = Builders<JsonItem>.Filter.Eq(x => x.Id, id);
            var existingItem = await _collection.Find(filter).FirstOrDefaultAsync(ct);

            var item = new JsonItem
            {
                Id = id,
                CreatedAt = existingItem?.CreatedAt ?? DateTime.UtcNow,
                Payload = payload
            };

            // Use ReplaceOne with upsert to update if exists, create if not
            var options = new ReplaceOptions { IsUpsert = true };
            await _collection.ReplaceOneAsync(filter, item, options, ct);

            return id;
        }

        public async Task<BsonDocument?> GetAsync(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var filter = Builders<JsonItem>.Filter.Eq(x => x.Id, id);
            var item = await _collection.Find(filter).FirstOrDefaultAsync(ct);
            
            return item?.Payload;
        }

        public async Task<bool> CheckConnectionAsync(CancellationToken ct = default)
        {
            try
            {
                // Ping the database to check connection
                await _database.RunCommandAsync<BsonDocument>(
                    new BsonDocument("ping", 1),
                    cancellationToken: ct);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}


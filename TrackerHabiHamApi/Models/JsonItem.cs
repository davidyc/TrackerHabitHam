using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrackerHabiHamApi.Models
{
    public class JsonItem
    {
        [BsonId]
        public string Id { get; set; } = null!;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("payload")]
        public BsonDocument Payload { get; set; } = null!;
    }
}


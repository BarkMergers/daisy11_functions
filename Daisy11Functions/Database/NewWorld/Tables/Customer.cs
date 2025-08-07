using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Daisy11Functions.Database.NewWorld.Tables
{
    public class Customer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId iid { get; set; }
        public long id { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public int age { get; set; }
        public bool active { get; set; }
    }
}
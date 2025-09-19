using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Daisy11Functions.Database.NewWorld.Tables
{
    public class Customer
    {
       // [BsonId]
       // [BsonRepresentation(BsonType.ObjectId)]
       // public ObjectId iid { get; set; }
        public long id { get; set; }
        public string? vehicle { get; set; }
        public DateTime? increasedate { get; set; }
        public string? fineoperator { get; set; }
        public decimal fineamount { get; set; }
        public decimal age { get; set; }
        public decimal power { get; set; }
        public string? issuer { get; set; }
        public string? status { get; set; }
    }

    public class CustomerIn
    {
        [BsonElement("iid")]
        public Iid? iid { get; set; }

        [BsonId]
        public long id { get; set; }
        public string? vehicle { get; set; }
        public string? increasedate { get; set; }
        public string? fineoperator { get; set; }
        public decimal fineamount { get; set; }
        public decimal age { get; set; }
        public decimal power { get; set; }
        public string? issuer { get; set; }
        public string? status { get; set; }
    }


    public class Iid
    {
        public long timestamp { get; set; }  // Represents the Unix timestamp (long)
        public DateTime creationTime { get; set; }  // Represents the ISO 8601 datetime string
    }



}
using Contracts.Domains;
using MongoDB.Bson.Serialization.Attributes;

namespace Inventory.Grpc.Entities;

public class InventoryEntry: MongoEntity
{
    [BsonElement("itemNo")]
    public string ItemNo { get; set; }
    
    [BsonElement("quantity")]
    public int Quantity { get; set; }
}
using Contracts.Domains;
using Infrastructure.Extensions;
using MongoDB.Bson.Serialization.Attributes;

namespace Inventory.Grpc.Entities;

[BsonCollection("InventoryEntries")]
public class InventoryEntry: MongoEntity
{
    [BsonElement("itemNo")]
    public string ItemNo { get; set; }
    
    [BsonElement("quantity")]
    public int Quantity { get; set; }
}
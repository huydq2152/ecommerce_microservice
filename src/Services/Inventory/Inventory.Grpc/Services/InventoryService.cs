using Grpc.Core;
using Inventory.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Inventory.Grpc.Services;

using Protos;

public class InventoryService : StockProtoService.StockProtoServiceBase
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger _logger;

    public InventoryService(IInventoryRepository inventoryRepository, ILogger logger)
    {
        _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<StockModel> GetStock(GetStockRequest request, ServerCallContext context)
    {
        _logger.Information($"Begin get stock of item no: {request.ItemNo}");
        var stockQuantity = await _inventoryRepository.GetStockQuantity(request.ItemNo);
        var result = new StockModel()
        {
            Quantity = stockQuantity
        };
        _logger.Information($"End get stock of item no: {request.ItemNo} - Quantity: {result.Quantity}");
        return result;
    }
}
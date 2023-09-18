using Inventory.Grpc.Client;
using ILogger = Serilog.ILogger;

namespace Basket.API.GrpcServices;

public class StockItemGrpcService
{
    private readonly StockProtoService.StockProtoServiceClient _stockProtoServiceClient;
    private readonly ILogger _logger;

    public StockItemGrpcService(StockProtoService.StockProtoServiceClient stockProtoServiceClient, ILogger logger)
    {
        _stockProtoServiceClient =
            stockProtoServiceClient ?? throw new ArgumentNullException(nameof(stockProtoServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<StockModel> GetStock(string itemNo)
    {
        try
        {
            var stockItemRequest = new GetStockRequest() { ItemNo = itemNo };
            return await _stockProtoServiceClient.GetStockAsync(stockItemRequest);
        }
        catch (Exception e)
        {
            _logger.Error(e,"GetStock method has error");
            throw;
        }
    }
}
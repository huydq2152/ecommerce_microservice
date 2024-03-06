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
            _logger.Information("Begin: GetStock StockItemGrpcService itemNo: {itemNo}", itemNo);
            var stockItemRequest = new GetStockRequest() { ItemNo = itemNo };
            var result = await _stockProtoServiceClient.GetStockAsync(stockItemRequest);
            _logger.Information("End: GetStock StockItemGrpcService itemNo: {itemNo} - stock result: {quantity}", itemNo, result.Quantity);
            return result;
        }
        catch (Exception e)
        {
            _logger.Error(e,"StockItemGrpcService GetStock Error: {Message}", e.Message);
            throw;
        }
    }
}
namespace Inventory.API.Extensions;

public class DatabaseSettings: Shared.Configuration.DatabaseSettings
{
    public string DatabaseName { get; set; }
}
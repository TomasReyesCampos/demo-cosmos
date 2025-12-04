using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SDDev.Net.GenericRepository.CosmosDB.Utilities;

namespace DemoCosmos.ConsoleApp.Infrastructure;

public class CosmosInitializer
{
    private readonly CosmosClient _client;
    private readonly IOptions<CosmosDbConfiguration> _config;
    private readonly ILogger<CosmosInitializer> _logger;

    public CosmosInitializer(
        CosmosClient client,
        IOptions<CosmosDbConfiguration> config,
        ILogger<CosmosInitializer> logger)
    {
        _client = client;
        _config = config;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var dbId = _config.Value.DefaultDatabaseName;

        try
        {
            // Verify database connection
            var database = _client.GetDatabase(dbId);
            var properties = await database.ReadAsync(cancellationToken: cancellationToken);

            _logger.LogInformation("✅ Connected to existing database: {DatabaseId}", dbId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error connecting to Cosmos DB database: {DatabaseId}", dbId);
            throw;
        }
    }
}
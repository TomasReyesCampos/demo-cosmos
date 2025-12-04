using DemoCosmos.ConsoleApp.Infrastructure;
using DemoCosmos.ConsoleApp.Models;
using DemoCosmos.ConsoleApp.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SDDev.Net.GenericRepository.Contracts.Repository;
using SDDev.Net.GenericRepository.CosmosDB;
using SDDev.Net.GenericRepository.CosmosDB.Utilities;

// Build configuration
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Setup DI container
var services = new ServiceCollection();
var entities = "entities";

// Register configuration
services.AddSingleton<IConfiguration>(config);

// Register Cosmos configuration using the configuration section
services.Configure<CosmosDbConfiguration>(config.GetSection("Cosmos"));

// Register CosmosClient
services.AddSingleton<CosmosClient>(sp =>
{
    var cosmosConfig = sp.GetRequiredService<IOptions<CosmosDbConfiguration>>().Value;
    return new CosmosClient(cosmosConfig.ConnectionString, new CosmosClientOptions
    {
        ApplicationName = "DemoCosmos.ConsoleApp"
    });
});

// Use SDDev Cosmos integration
services.UseCosmosDb(config);

// Register repositories
services.AddTransient<IRepository<Account>>(sp =>
{
    var client = sp.GetRequiredService<CosmosClient>();
    var logger = sp.GetRequiredService<ILogger<GenericRepository<Account>>>();
    var opts = sp.GetRequiredService<IOptions<CosmosDbConfiguration>>();
    return new GenericRepository<Account>(client, logger, opts, entities);
});

services.AddTransient<IRepository<Player>>(sp =>
{
    var client = sp.GetRequiredService<CosmosClient>();
    var logger = sp.GetRequiredService<ILogger<GenericRepository<Player>>>();
    var opts = sp.GetRequiredService<IOptions<CosmosDbConfiguration>>();
    return new GenericRepository<Player>(client, logger, opts, entities);
});

services.AddTransient<IRepository<Team>>(sp =>
{
    var client = sp.GetRequiredService<CosmosClient>();
    var logger = sp.GetRequiredService<ILogger<GenericRepository<Team>>>();
    var opts = sp.GetRequiredService<IOptions<CosmosDbConfiguration>>();
    return new GenericRepository<Team>(client, logger, opts, entities);
});

services.AddTransient<IRepository<Coach>>(sp =>
{
    var client = sp.GetRequiredService<CosmosClient>();
    var logger = sp.GetRequiredService<ILogger<GenericRepository<Coach>>>();
    var opts = sp.GetRequiredService<IOptions<CosmosDbConfiguration>>();
    return new GenericRepository<Coach>(client, logger, opts, entities);
});

services.AddTransient<IRepository<Game>>(sp =>
{
    var client = sp.GetRequiredService<CosmosClient>();
    var logger = sp.GetRequiredService<ILogger<GenericRepository<Game>>>();
    var opts = sp.GetRequiredService<IOptions<CosmosDbConfiguration>>();
    return new GenericRepository<Game>(client, logger, opts, entities);
});

// Register seeding services
services.AddSingleton<CosmosInitializer>();
services.AddTransient<DemoDataSeeder>();

// Setup logging
services.AddLogging(builder =>
{
    builder.AddConfiguration(config.GetSection("Logging"));
    builder.AddConsole();
});

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Main execution
ILogger<Program>? logger = null;
try
{
    using (var scope = serviceProvider.CreateScope())
    {
        logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("🚀 Demo Cosmos Console App Started");

        // Initialize database
        var initializer = scope.ServiceProvider.GetRequiredService<CosmosInitializer>();
        await initializer.InitializeAsync(CancellationToken.None);
        logger.LogInformation("✅ Database initialized");

        // Run demo seeding
        var seeder = scope.ServiceProvider.GetRequiredService<DemoDataSeeder>();
        await seeder.SeedDemoDataAsync(CancellationToken.None);
        logger.LogInformation("✅ Demo data seeded successfully");
    }
}
catch (Exception ex)
{
    logger ??= serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "❌ Error during execution");
    Environment.Exit(1);
}

logger ??= serviceProvider.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🏁 Application completed");
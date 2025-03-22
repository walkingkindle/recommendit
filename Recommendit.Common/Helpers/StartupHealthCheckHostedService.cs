 using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using Recommendit.Infrastructure;
using MongoDB.Bson;
namespace Recommendit.Common.Helpers
{


public class StartupHealthCheckService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StartupHealthCheckService> _logger;

    public StartupHealthCheckService(IServiceProvider serviceProvider, ILogger<StartupHealthCheckService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShowContext>(); // SQL
        var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>(); // MongoDB
        
        try
        {
            _logger.LogInformation("Checking SQL Server connection...");
            await dbContext.Database.CanConnectAsync(cancellationToken);
            _logger.LogInformation("SQL Server connection successful.");

            _logger.LogInformation("Checking MongoDB connection...");
            var database = mongoClient.GetDatabase("Recommendit"); 
            await database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
            _logger.LogInformation("MongoDB connection successful.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Database connection check failed. Terminating application.");
            throw new ApplicationException("Database connection check failed", ex);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

 }

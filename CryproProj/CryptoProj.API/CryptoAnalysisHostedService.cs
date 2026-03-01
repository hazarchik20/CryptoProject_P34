using CryptoProj.Domain.Abstractions;
using CryptoProj.Domain.Models.Requests;

namespace CryptoProj.API;

public class CryptoAnalysisHostedService : IHostedService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<CryptoAnalysisHostedService> _logger;

    public CryptoAnalysisHostedService(IServiceProvider serviceProvider, ILogger<CryptoAnalysisHostedService> logger)
    {
        _provider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _provider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICryptocurrencyRepository>();
            
            var request = new CryptocurrencyRequest
            {
                Limit = 100,
                Offset = 1
            };
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var cryptos = await repo.GetAll(request);
                
                var lowCryptos = cryptos.Where(x => x.Price < 0.005m);
                
                _logger.LogWarning($"cryptos with low prices: {string.Join(",", lowCryptos.Select(x => x.Symbol))}");
                
                await Task.Delay(10000, cancellationToken);
            }
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Task was cancelled");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
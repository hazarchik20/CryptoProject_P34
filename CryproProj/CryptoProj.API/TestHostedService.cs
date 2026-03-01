using System.Diagnostics;

namespace CryptoProj.API;

public class TestHostedService : IHostedService
{
    private Timer _timer;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var processesCount = Process.GetProcesses().Length;
            Console.WriteLine($"Processes count: {processesCount}");
            await Task.Delay(1000);
        }
        
        //_timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("service is stopping...");
        //await _timer.DisposeAsync();
    }
    
    private void TimerCallback(object? state)
    {
        Console.WriteLine(DateTime.UtcNow.ToLongTimeString());
    }
}
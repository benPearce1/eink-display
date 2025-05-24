namespace app.Display;

public class DisplayWorker(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IScreen[] screens = serviceScopeFactory.CreateScope().ServiceProvider.GetServices<IScreen>().ToArray();
        
        int currentScreenIndex = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
            IScreen screen = screens[currentScreenIndex];
            Console.WriteLine($"Screen: {screen.Id} - {await screen.Execute()}");
            
            currentScreenIndex++;
            currentScreenIndex %= screens.Length;
            _ = screens[currentScreenIndex].Init(stoppingToken);
        }
    }
}
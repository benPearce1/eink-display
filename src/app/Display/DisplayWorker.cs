namespace app.Display;

public class DisplayWorker(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IScreen[] screens = serviceScopeFactory.CreateScope().ServiceProvider.GetServices<IScreen>().ToArray();

        foreach (IScreen screen in screens)
        {
            await screen.Init(stoppingToken);
        }

        
        int currentScreenIndex = 0;
        await screens[currentScreenIndex].Refresh(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
            IScreen screen = screens[currentScreenIndex];
            var image = await screen.Execute();
            await using var stream = File.OpenWrite("/Users/benpearce/eink-images/output.png");
            image?.SaveTo(stream);
            Console.WriteLine($"Screen: {screen.Id}");
            
            currentScreenIndex++;
            currentScreenIndex %= screens.Length;
            _ = screens[currentScreenIndex].Refresh(stoppingToken);
        }
    }
}
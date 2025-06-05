using SkiaSharp;

namespace app.Display;

public class Screen1 : IScreen
{
    public string Id => "Screen1";

    public async Task Init(CancellationToken cancellationToken)
    {
        Console.WriteLine("Initialising Screen1");
        await Task.Delay(1000, cancellationToken);
        Console.WriteLine("Initialised Screen1");
    }

    public Task Refresh(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task<SKData?> Execute()
    {
        await Task.CompletedTask;
        return null;
    }
}

public class Screen2 : IScreen
{
    public string Id => "Screen2";
    public async Task Init(CancellationToken cancellationToken)
    {
        Console.WriteLine("Initialising Screen2");
        await Task.Delay(1000, cancellationToken);
        Console.WriteLine("Initialised Screen2");
    }

    public Task Refresh(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task<SKData?> Execute()
    {
        await Task.CompletedTask;
        return null;
    }
}

public class Screen3 : IScreen
{
    public string Id => "Screen3";
    public async Task Init(CancellationToken cancellationToken)
    {
        Console.WriteLine("Initialising Screen3");
        await Task.Delay(1000, cancellationToken);
        Console.WriteLine("Initialised Screen3");
    }

    public Task Refresh(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task<SKData> Execute()
    {
        await Task.CompletedTask;
        return null;
    }
}
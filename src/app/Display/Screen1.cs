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

    public async Task<int> Execute()
    {
        await Task.CompletedTask;
        return 1;
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

    public async Task<int> Execute()
    {
        await Task.CompletedTask;
        return 2;
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

    public async Task<int> Execute()
    {
        await Task.CompletedTask;
        return 3;
    }
}

public interface IScreen
{
    string Id { get; }
    Task Init(CancellationToken cancellationToken);
    Task<int> Execute();
}
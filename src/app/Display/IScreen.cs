using SkiaSharp;

namespace app.Display;

public interface IScreen
{
    string Id { get; }
    Task Init(CancellationToken cancellationToken);
    
    Task Refresh(CancellationToken cancellationToken);
    Task<SKData?> Execute();
}
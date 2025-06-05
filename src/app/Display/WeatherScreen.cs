using System.Text.Json;
using SkiaSharp;

namespace app.Display;

public class WeatherScreen : IScreen
{
    static readonly HttpClient httpClient = new();

    static async Task<WeatherInfo> GetWeatherAsync(string city, string apiKey, CancellationToken cancellationToken)
    {
        string url = $"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}&aqi=no";
        var response = await httpClient.GetStringAsync(url, cancellationToken);
        using var doc = JsonDocument.Parse(response);

        var root = doc.RootElement;
        var location = root.GetProperty("location").GetProperty("name").GetString();
        var region = root.GetProperty("location").GetProperty("region").GetString();
        var tempC = root.GetProperty("current").GetProperty("temp_c").GetDecimal();
        var feelsLikeC = root.GetProperty("current").GetProperty("feelslike_c").GetDecimal();
        var humidity = root.GetProperty("current").GetProperty("humidity").GetInt32();
        var windKph = root.GetProperty("current").GetProperty("wind_kph").GetDecimal();
        var condition = root.GetProperty("current").GetProperty("condition");
        var conditionText = condition.GetProperty("text").GetString();
        var iconUrl = "https:" + condition.GetProperty("icon").GetString();

        return new WeatherInfo
        {
            Location = $"{location}, {region}",
            Description = conditionText,
            IconUrl = iconUrl,
            Temperature = tempC,
            FeelsLike = feelsLikeC,
            Humidity = humidity,
            WindSpeed = windKph,
            Time = DateTime.Now
        };
    }

    static SKData DrawWeatherImage(WeatherInfo weather, SKBitmap? icon)
    {
        using var surface = SKSurface.Create(new SKImageInfo(Constants.CanvasWidth, Constants.CanvasHeight));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.DarkSlateGray);

        using var paint = new SKPaint
        {
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("DejaVu Sans", SKFontStyle.Bold),
            TextSize = 48,
            Color = SKColors.White,
            TextAlign = SKTextAlign.Left
        };

        int margin = 40;
        int lineHeight = 60;
        float y = margin;

        canvas.DrawText($"Weather in {weather.Location}", margin, y, paint);
        y += lineHeight * 1.5f;

        // Draw icon if available
        if (icon != null)
        {
            canvas.DrawBitmap(icon, new SKRect(margin, y, margin + 64, y + 64));
        }

        paint.TextSize = 42;
        float textStartX = margin + (icon != null ? 80 : 0);

        canvas.DrawText($"{weather.Description}", textStartX, y + 48, paint);
        y += lineHeight;

        canvas.DrawText($"Temp: {weather.Temperature}°C (Feels like {weather.FeelsLike}°C)", margin, y, paint);
        y += lineHeight;
        canvas.DrawText($"Humidity: {weather.Humidity}%", margin, y, paint);
        y += lineHeight;
        canvas.DrawText($"Wind: {weather.WindSpeed} km/h", margin, y, paint);
        y += lineHeight * 1.5f;

        paint.TextSize = 36;
        paint.Color = SKColors.LightGray;
        canvas.DrawText($"Updated: {weather.Time:HH:mm:ss}", margin, y, paint);

        using var img = surface.Snapshot();
        var data = img.Encode(SKEncodedImageFormat.Png, 100);
        return data;
        // using var stream = System.IO.File.OpenWrite(outputPath);
        // data.SaveTo(stream);
        // Console.WriteLine($"Saved image to: {outputPath}");
    }
    
    static async Task<SKBitmap?> DownloadWeatherIconAsync(string url)
    {
        try
        {
            var bytes = await httpClient.GetByteArrayAsync(url);
            return SKBitmap.Decode(bytes);
        }
        catch
        {
            return null;
        }
    }

    class WeatherInfo
    {
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public decimal Temperature { get; set; }
        public decimal FeelsLike { get; set; }
        public int Humidity { get; set; }
        public decimal WindSpeed { get; set; }
        public DateTime Time { get; set; }
    }

    string city = "Brisbane"; // or "auto:ip" for IP geolocation
    string apiKey = ""; // WeatherAPI key
    private WeatherInfo latest;
    private SKBitmap? icon;

    public string Id => "Weather";
    public async Task Init(CancellationToken cancellationToken)
    {
        latest = await GetWeatherAsync(city, apiKey, cancellationToken);
        icon = await DownloadWeatherIconAsync(latest.IconUrl);
    }

    public async Task Refresh(CancellationToken cancellationToken)
    {
        var updated = await GetWeatherAsync(city, apiKey, cancellationToken);
        icon = await DownloadWeatherIconAsync(latest.IconUrl);
    }

    public async Task<SKData?> Execute()
    {
        await Task.CompletedTask;
        return DrawWeatherImage(latest, icon);
    }
}
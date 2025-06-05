using System;
using System.Collections.Generic;
using SkiaSharp;


namespace app.Display;

public class MaverickClockScreen : IScreen
{
    static readonly string[] Grid = new[]
    {
        "ITLISASTIME",
        "ACQUARTERDC",
        "TWENTYFIVEX",
        "HALFBTENFTO",
        "PASTERUNINE",
        "ONESIXTHREE",
        "FOURFIVETWO",
        "EIGHTELEVEN",
        "SEVENTWELVE",
        "TENSEOCLOCK"
    };

    static readonly Dictionary<string, (int row, int col, int length)> WordMap = new()
    {
        { "IT",        (0, 0, 2) },
        { "IS",        (0, 3, 2) },
        { "A",         (0, 6, 1) },
        { "QUARTER",   (1, 3, 7) },
        { "TWENTY",    (2, 0, 6) },
        { "FIVE",      (2, 6, 4) },
        { "HALF",      (3, 0, 4) },
        { "TEN",       (3, 5, 3) },
        { "TO",        (3, 9, 2) },
        { "PAST",      (4, 0, 4) },
        { "ONE",       (5, 0, 3) },
        { "TWO",       (6, 9, 3) },
        { "THREE",     (5, 6, 5) },
        { "FOUR",      (6, 0, 4) },
        { "FIVE_H",    (6, 4, 4) },
        { "SIX",       (5, 3, 3) },
        { "SEVEN",     (8, 0, 5) },
        { "EIGHT",     (7, 0, 5) },
        { "NINE",      (4, 7, 4) },
        { "TEN_H",     (9, 0, 3) },
        { "ELEVEN",    (7, 5, 6) },
        { "TWELVE",    (8, 5, 6) },
        { "OCLOCK",    (9, 5, 6) },
    };

    static string[] GetActiveWords(int hour, int minute)
    {
        var active = new List<string> { "IT", "IS" };

        int roundedMinute = (int)(5 * Math.Round(minute / 5.0));

        if (roundedMinute == 0)
        {
            active.Add(GetHourWord(hour));
            active.Add("OCLOCK");
        }
        else
        {
            if (roundedMinute <= 30)
            {
                if (roundedMinute == 15) active.Add("QUARTER");
                else if (roundedMinute == 20) active.Add("TWENTY");
                else if (roundedMinute == 25) { active.Add("TWENTY"); active.Add("FIVE"); }
                else if (roundedMinute == 30) active.Add("HALF");
                else if (roundedMinute == 5) active.Add("FIVE");
                else if (roundedMinute == 10) active.Add("TEN");

                active.Add("PAST");
                active.Add(GetHourWord(hour));
            }
            else
            {
                int minutesTo = 60 - roundedMinute;
                int nextHour = (hour + 1) % 12;

                if (minutesTo == 15) active.Add("QUARTER");
                else if (minutesTo == 20) active.Add("TWENTY");
                else if (minutesTo == 25) { active.Add("TWENTY"); active.Add("FIVE"); }
                else if (minutesTo == 5) active.Add("FIVE");
                else if (minutesTo == 10) active.Add("TEN");

                active.Add("TO");
                active.Add(GetHourWord(nextHour));
            }
        }

        return active.ToArray();
    }

    static string GetHourWord(int hour)
    {
        if (hour > 12) hour -= 12;
        return hour switch
        {
            1 => "ONE",
            2 => "TWO",
            3 => "THREE",
            4 => "FOUR",
            5 => "FIVE_H",
            6 => "SIX",
            7 => "SEVEN",
            8 => "EIGHT",
            9 => "NINE",
            10 => "TEN_H",
            11 => "ELEVEN",
            0 => "TWELVE",
            12 => "TWELVE",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static void GenerateClockImage(DateTime time, string outputPath = "maverick_clock_skia.png")
    {
        int width = 800, height = 480;
        int rows = Grid.Length, cols = Grid[0].Length;

        var activeWords = GetActiveWords(time.Hour, time.Minute);
        var activePositions = new HashSet<(int row, int col)>();

        foreach (var word in activeWords)
        {
            if (!WordMap.TryGetValue(word, out var pos)) continue;
            for (int i = 0; i < pos.length; i++)
                activePositions.Add((pos.row, pos.col + i));
        }

        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Black);

        int cellWidth = width / cols;
        int cellHeight = height / rows;
        int fontSize = (int)(Math.Min(cellWidth, cellHeight) * 0.8);

        using var paint = new SKPaint
        {
            IsAntialias = true,
            TextSize = fontSize,
            Typeface = SKTypeface.FromFamilyName("DejaVu Sans Mono", SKFontStyle.Bold),
            TextAlign = SKTextAlign.Center
        };

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                char c = Grid[row][col];
                paint.Color = activePositions.Contains((row, col)) ? SKColors.White : SKColors.DimGray;

                float x = col * cellWidth + cellWidth / 2f;
                float y = row * cellHeight + cellHeight * 0.75f; // adjust vertical alignment

                canvas.DrawText(c.ToString(), x, y, paint);
            }
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = System.IO.File.OpenWrite(outputPath);
        data.SaveTo(stream);

        Console.WriteLine($"Saved word clock to: {outputPath}");
    }


    public string Id => "MaverickClockScreen";
    public Task Init(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<int> Execute()
    {
        GenerateClockImage(DateTime.Now, "/Users/benpearce/MaverickClockScreen.png");
        return Task.FromResult(0);
    }
}
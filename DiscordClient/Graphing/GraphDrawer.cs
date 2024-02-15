using System.Reflection;
using OsrsFlipper.Data.TimeSeries;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using Path = System.IO.Path;

namespace DiscordClient.Graphing;

public static class GraphDrawer
{
    private static readonly string GraphBackgroundPath;
    private static readonly Pen GraphingPenLow;
    private static readonly Pen GraphingPenHigh;
    private static readonly Pen Last6HPen;
    private static readonly Font Font;


    static GraphDrawer()
    {
        const string fontPath = @"assets\fonts\runescape_uf.ttf";
        const string graphBackgroundPath = @"assets\textures\graph_background.png";
        
        string executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        GraphBackgroundPath = Path.Combine(executingAssemblyPath, graphBackgroundPath);
        string fontPath1 = Path.Combine(executingAssemblyPath, fontPath);
        
        FontCollection fontCollection = new();
        FontFamily family = fontCollection.Add(fontPath1);
        Font = new Font(family, 14, FontStyle.Regular);
        
        GraphingPenLow = new SolidPen(new SolidBrush(Color.Chartreuse), 1f);
        GraphingPenHigh = new SolidPen(new SolidBrush(Color.Orange), 1f);
        Last6HPen = new SolidPen(new SolidBrush(Color.Brown), 3f);
    }


    /// <summary>
    /// Draws a price graph from the price history of an item.
    /// </summary>
    /// <param name="history5MinIntervals">Item price history, in 5min intervals. Should contain at least 288x 5min data-points = 24h.</param>
    /// <param name="latestBuy">Latest buy price.</param>
    /// <param name="latestSell">Latest sell price.</param>
    public static async Task<MemoryStream> DrawGraph(ItemPriceHistory history5MinIntervals, int latestBuy, int latestSell)
    {
        if (history5MinIntervals.Data.Count < 288)
            throw new ArgumentException("The price history must contain at least 288x 5min data-points = 24h.", nameof(history5MinIntervals));
        List<ItemPriceHistoryEntry> dataPoints = history5MinIntervals.Data.TakeLast(288).ToList();
        dataPoints.Add(new ItemPriceHistoryEntry
        {
            AvgHighPrice = latestBuy,
            AvgLowPrice = latestSell,
        });

        // 460x80 pixels.
        using Image img = await Image.LoadAsync(GraphBackgroundPath);
        int imageWidth = img.Width;
        int imageHeight = img.Height;

        // Get the lowest and highest price values in the history.
        int minValue = int.MaxValue;
        int maxValue = int.MinValue;
        foreach (ItemPriceHistoryEntry entry in dataPoints)
        {
            if (entry.LowestPrice == null)
                continue;
            if ((int)entry.LowestPrice < minValue)
                minValue = (int)entry.LowestPrice;
            
            if (entry.HighestPrice == null)
                continue;
            if ((int)entry.HighestPrice > maxValue)
                maxValue = (int)entry.HighestPrice;
        }

        List<PointF> graphPointsLow = new();
        List<PointF> graphPointsHigh = new();

        float last6HStartPoint = imageWidth - (imageWidth - 15) / 4f;
        for (int i = 0; i < dataPoints.Count; i++)
        {
            if (dataPoints[i].AvgLowPrice != null)
            {
                PointF point = NormalizedPointFromValues(i, (int)dataPoints[i].AvgLowPrice!, imageWidth, imageHeight, 15, 15, dataPoints.Count, minValue, maxValue);

                if (i == 288 - 72)
                    last6HStartPoint = point.X;

                graphPointsLow.Add(point);
            }
            
            if (dataPoints[i].AvgHighPrice != null)
            {
                PointF point = NormalizedPointFromValues(i, (int)dataPoints[i].AvgHighPrice!, imageWidth, imageHeight, 15, 15, dataPoints.Count, minValue, maxValue);

                if (i == 288 - 72)
                    last6HStartPoint = point.X;
                
                graphPointsHigh.Add(point);
            }
        }
        
        PathBuilder graphPathLow = new();
        PathBuilder graphPathHigh = new();
        graphPathLow.AddLines(graphPointsLow.ToArray());
        graphPathHigh.AddLines(graphPointsHigh.ToArray());
        
        // Draw the graphs.
        img.Mutate(ctx => ctx.Draw(GraphingPenLow, graphPathLow.Build()));
        img.Mutate(ctx => ctx.Draw(GraphingPenHigh, graphPathHigh.Build()));
        
        // Construct points for the 6h helper line.
        List<PointF> last6HPoints = new();
        int targetHeight6H = imageHeight - 15;
        int latestPoint = imageWidth - 15;
        
        last6HPoints.Add(new PointF(last6HStartPoint, targetHeight6H));
        last6HPoints.Add(new PointF(latestPoint, targetHeight6H));

        // Draw the 6h helper line
        PathBuilder helperPath = new();
        helperPath.AddLines(last6HPoints.ToArray());
        img.Mutate(ctx => ctx.Draw(Last6HPen, helperPath.Build()));
        
        // Draw the "last 6h ->" text.
        img.Mutate(ctx => ctx.DrawText("last 6h ->", Font, Color.Wheat, new PointF(last6HStartPoint - 60, imageHeight - 20)));

        MemoryStream ms = new();
        await img.SaveAsync(ms, new PngEncoder());
        ms.Position = 0;
        return ms;
    }


    private static PointF NormalizedPointFromValues(int pointIndex, int pointValue, int imageWidth, int imageHeight, int widthPadding, int heightPadding, int pointsCount, int lowestValue, int highestValue)
    {
        float normalizedX = pointIndex * (imageWidth - widthPadding - widthPadding) / (float)(pointsCount - 1) + widthPadding;
        float normalizedY = (pointValue - lowestValue) * (imageHeight - heightPadding - heightPadding) / (float)(highestValue - lowestValue) + heightPadding;

        // Invert the Y-axis.
        normalizedY = imageHeight - normalizedY;

        return new PointF(normalizedX, normalizedY);
    }
}
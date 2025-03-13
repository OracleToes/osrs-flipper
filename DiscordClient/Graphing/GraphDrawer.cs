using System.Reflection;
using MathNet.Numerics;
using OsrsFlipper.Data.TimeSeries;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Path = System.IO.Path;

namespace DiscordClient.Graphing;

public static class GraphDrawer
{
    private enum GraphTimePeriod
    {
        DAY,
        MONTH
    }

    private static readonly string GraphBackgroundPath;
    private static readonly Pen GraphingPenLow;
    private static readonly Pen GraphingPenHigh;
    private static readonly Pen HighlightingPen;
    private static readonly Pen RegressionPen;
    private static readonly Font Font;


    static GraphDrawer()
    {
        string fontPath = Path.Combine("assets", "fonts", "runescape_uf.ttf");
        string graphBackgroundPath = Path.Combine("assets", "textures", "graph_background.png");

        string executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        GraphBackgroundPath = Path.Combine(executingAssemblyPath, graphBackgroundPath);
        string fPath = Path.Combine(executingAssemblyPath, fontPath);

        FontCollection fontCollection = new();
        FontFamily family = fontCollection.Add(fPath);
        Font = new Font(family, 14, FontStyle.Regular);

        GraphingPenLow = new SolidPen(new SolidBrush(Color.Lime), 1f);
        GraphingPenHigh = new SolidPen(new SolidBrush(Color.Crimson), 1f);
        HighlightingPen = new SolidPen(new SolidBrush(Color.CadetBlue), 3f);
        RegressionPen = new SolidPen(new SolidBrush(Color.Gold), 1f);
    }


    /// <summary>
    /// Draws a price graph from the price history of an item.
    /// </summary>
    /// <param name="history5MinIntervals">Item price history, in 5min intervals. Should contain at least 288x 5min data-points = 24h.</param>
    /// <param name="history6HourIntervals">Item price history, in 6h intervals. May be null if not available.</param>
    /// <param name="latestBuy">Latest buy price.</param>
    /// <param name="latestSell">Latest sell price.</param>
    public static async Task<MemoryStream> DrawGraph(
        ItemPriceHistory history5MinIntervals,
        ItemPriceHistory? history6HourIntervals,
        int latestBuy,
        int latestSell)
    {
        const int graphPointsPerDay = 288; // Assuming 5min intervals.

        if (history5MinIntervals.Data.Count < graphPointsPerDay)
            throw new ArgumentException("The price history must contain at least 288x 5min data-points = 24h.", nameof(history5MinIntervals));

        List<ItemPriceHistoryEntry> dataPointsDay = history5MinIntervals.Data.TakeLast(graphPointsPerDay).ToList();
        dataPointsDay.Add(
            new ItemPriceHistoryEntry
            {
                AvgHighPrice = latestBuy,
                AvgLowPrice = latestSell
            });

        Image dayGraph = await CreateGraph(dataPointsDay, GraphTimePeriod.DAY);
        Image? monthGraph = null;

        if (history6HourIntervals != null)
        {
            List<ItemPriceHistoryEntry> dataPointsMonth = history6HourIntervals.Data.TakeLast(120).ToList();
            dataPointsMonth.Add(
                new ItemPriceHistoryEntry
                {
                    AvgHighPrice = latestBuy,
                    AvgLowPrice = latestSell
                });

            monthGraph = await CreateGraph(dataPointsMonth, GraphTimePeriod.MONTH);
        }

        int width = Math.Max(dayGraph.Width, monthGraph?.Width ?? 0);
        int height = dayGraph.Height + (monthGraph?.Height ?? 0);
        Image combinedImage = new Image<Rgba32>(width, height);
        combinedImage.Mutate(ctx => ctx.DrawImage(dayGraph, new Point(0, 0), 1f));
        if (monthGraph != null)
            combinedImage.Mutate(ctx => ctx.DrawImage(monthGraph, new Point(0, dayGraph.Height), 1f));

        MemoryStream ms = new();
        await combinedImage.SaveAsync(ms, new PngEncoder());
        ms.Position = 0;

        combinedImage.Dispose();

        return ms;
    }


    private static async Task<Image> CreateGraph(List<ItemPriceHistoryEntry> dataPoints, GraphTimePeriod timePeriod)
    {
        const int graphPadding = 15;

        // 460x80 pixels.
        Image img = await Image.LoadAsync(GraphBackgroundPath);
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

        // Determine required values.
        float periodHighlightStartPoint = imageWidth - (imageWidth - graphPadding) / 4f;
        int periodCount;
        int periodCountForStartPoint;
        string periodText;
        string lastPeriodText;
        switch (timePeriod)
        {
            case GraphTimePeriod.DAY:
                periodCount = 288;
                periodCountForStartPoint = 73;
                periodText = "last 24h";
                lastPeriodText = "last 6h ->";
                break;
            case GraphTimePeriod.MONTH:
                periodCount = 120;
                periodCountForStartPoint = 5;
                periodText = "last 30d";
                lastPeriodText = "last day ->";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(timePeriod), timePeriod, null);
        }

        // Construct the graph points.
        List<PointF> graphPointsLow = new();
        List<PointF> graphPointsHigh = new();
        for (int i = 0; i < dataPoints.Count; i++)
        {
            if (dataPoints[i].AvgLowPrice != null)
            {
                PointF point = NormalizedPointFromValues(
                    i,
                    (int)dataPoints[i].AvgLowPrice!,
                    imageWidth,
                    imageHeight,
                    graphPadding,
                    graphPadding,
                    dataPoints.Count,
                    minValue,
                    maxValue);

                if (i == periodCount - periodCountForStartPoint)
                    periodHighlightStartPoint = point.X;

                graphPointsLow.Add(point);
            }

            if (dataPoints[i].AvgHighPrice != null)
            {
                PointF point = NormalizedPointFromValues(
                    i,
                    (int)dataPoints[i].AvgHighPrice!,
                    imageWidth,
                    imageHeight,
                    graphPadding,
                    graphPadding,
                    dataPoints.Count,
                    minValue,
                    maxValue);

                if (i == periodCount - periodCountForStartPoint)
                    periodHighlightStartPoint = point.X;

                graphPointsHigh.Add(point);
            }
        }

        // Construct the graph paths.
        PathBuilder graphPathLow = new PathBuilder().AddLines(graphPointsLow.ToArray());
        PathBuilder graphPathHigh = new PathBuilder().AddLines(graphPointsHigh.ToArray());
        
        // Construct the regression path.
        PathBuilder regressionPath = DrawRegressionLine(dataPoints, imageWidth, imageHeight, graphPadding, minValue, maxValue);
        
        // Draw the regression curve.
        img.Mutate(ctx => ctx.Draw(RegressionPen, regressionPath.Build()));

        // Draw the graphs.
        img.Mutate(ctx => ctx.Draw(GraphingPenLow, graphPathLow.Build()));
        img.Mutate(ctx => ctx.Draw(GraphingPenHigh, graphPathHigh.Build()));

        // Construct points for the helper line.
        List<PointF> highlightedPeriodPoints = new();
        int targetHeightPeriod = imageHeight - graphPadding;
        int latestPoint = imageWidth - graphPadding;

        // Add the last period points.
        highlightedPeriodPoints.Add(new PointF(periodHighlightStartPoint, targetHeightPeriod));
        highlightedPeriodPoints.Add(new PointF(latestPoint, targetHeightPeriod));

        // Draw the helper line
        PathBuilder helperPath = new();
        helperPath.AddLines(highlightedPeriodPoints.ToArray());
        img.Mutate(ctx => ctx.Draw(HighlightingPen, helperPath.Build()));

        // Draw the "last period ->" text.
        img.Mutate(ctx => ctx.DrawText(lastPeriodText, Font, Color.Wheat, new PointF(periodHighlightStartPoint - 60, imageHeight - graphPadding - 5)));

        // Add the "period" text.
        img.Mutate(ctx => ctx.DrawText(periodText, Font, Color.Wheat, new PointF(10, 5)));

        return img;
    }


    private static PathBuilder DrawRegressionLine(List<ItemPriceHistoryEntry> dataPoints, int imageWidth, int imageHeight, int graphPadding, int minValue, int maxValue)
    {
        // Prepare data for polynomial regression
        double[] xValues = dataPoints.Select((v, i) => (double)i).ToArray();
        int previous = dataPoints[0].AvgHighPrice ?? 0;
        double[] yValues = dataPoints.Select(v =>
        {
            int prev = previous;
            previous = v.AvgHighPrice ?? previous;
            if (v.AvgHighPrice != null)
                return (double)v.AvgHighPrice;
            return previous;
        }).ToArray();

        // Fit data to a 2nd degree polynomial (change the 2 to a higher number for a higher degree polynomial)
        double[] coefficients = Fit.Polynomial(xValues, yValues, 2);

        // Create a function that represents the polynomial
        Func<double, double> polynomial = x => coefficients[0] + coefficients[1] * x + coefficients[2] * Math.Pow(x, 2);

        // Calculate points for the regression curve
        List<PointF> regressionPoints = xValues.Select(x => 
            NormalizedPointFromValues(
                (int)x, 
                (int)polynomial(x), 
                imageWidth, 
                imageHeight, 
                graphPadding, 
                graphPadding, 
                dataPoints.Count, 
                minValue, 
                maxValue)
        ).ToList();

        // Construct the regression path
        PathBuilder regressionPath = new();
        regressionPath.AddLines(regressionPoints.ToArray());

        return regressionPath;
    }


    /// <summary>
    /// Normalizes a data point to a point on the graph.
    /// </summary>
    private static PointF NormalizedPointFromValues(
        int pointIndex,
        int pointValue,
        int imageWidth,
        int imageHeight,
        int widthPadding,
        int heightPadding,
        int pointsCount,
        int lowestValue,
        int highestValue)
    {
        float normalizedX = pointIndex * (imageWidth - widthPadding - widthPadding) / (float)(pointsCount - 1) + widthPadding;
        float normalizedY = (pointValue - lowestValue) * (imageHeight - heightPadding - heightPadding) / (float)(highestValue - lowestValue) + heightPadding;

        // Invert the Y-axis.
        normalizedY = imageHeight - normalizedY;

        return new PointF(normalizedX, normalizedY);
    }
}

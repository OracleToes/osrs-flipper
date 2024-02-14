using System.Text;
using Discord;
using OsrsFlipper;

namespace DiscordClient;

internal static class DumpEmbedBuilder
{
    private static bool colorFlipFlop;


    public static Embed BuildEmbed(ItemDump dump, string graphUrl)
    {
        EmbedBuilder builder = new()
        {
            Title = $":chart_with_downwards_trend: {dump.Item.Name.ToUpper()}",
            Url = dump.Item.OsrsWikiPricesLink,
            ImageUrl = graphUrl,
            Color = GetColor()
        };

        builder.AddField("> **[-LINKS-]**", GetLinks(), false);
        builder.WithThumbnailUrl(dump.Item.GetIconUrl());
        
        builder.AddField("> **[-CALCULATED-VALUES-]**", GetCalculatedData(), false);
        
        builder.AddField("> **[-PRICES-]**", GetPrices(), false);
        
        builder.WithFooter("osrs-flipper");
        builder.WithCurrentTimestamp();

        return builder.Build();

        string GetLinks()
        {
            // Build a string with all the links.
            StringBuilder sb = new();
            sb.Append($"*[WIKI]({dump.Item.OsrsWikiLink})");
            sb.Append($" | [WIKI-PRICES]({dump.Item.OsrsWikiPricesLink})");
            sb.Append($" | [GE-TRACKER]({dump.Item.GeTrackerLink})*");
            return sb.ToString();
        }

        string GetCalculatedData()
        {
            // Build a string with all the calculated data.
            StringBuilder sb = new();
            string profit = dump.PotentialProfit.HasValue ? $"{dump.PotentialProfit.Value.SeparateThousands()}" : "Unknown";
            string volume = dump.TotalVolume24H.SeparateThousands();
            string limit = dump.Item.HasBuyLimit ? dump.Item.GeBuyLimit.SeparateThousands() : "Unknown";

            sb.Append($"**Volume (24h):** *`{volume}`*\n");
            sb.Append($"**Limit:** *`{limit}`*\n");
            sb.Append($"**Max Profit:** *`{profit}` gp*\n");
            sb.Append($"**ROI:** *`{dump.RoiPercentage:F1}`%*");
            return sb.ToString();
        }

        string GetPrices()
        {
            // Build a string with all the prices.
            StringBuilder sb = new();
            sb.Append($"**Buy:** *`{dump.InstaBuyPrice.SeparateThousands()}` gp* <t:{Utils.DateTimeToUnixTime(dump.LastInstaBuyTime)}:R>\n");
            sb.Append($"**Sell:** *`{dump.InstaSellPrice.SeparateThousands()}` gp* <t:{Utils.DateTimeToUnixTime(dump.LastInstaSellTime)}:R>\n");
            sb.Append($"**30min Avg:** *`{dump.AveragePrice30Min.SeparateThousands()}` gp*\n");
            sb.Append($"**6h Avg:** *`{dump.AveragePrice6Hour.SeparateThousands()}` gp*");
            return sb.ToString();
        }

        Color GetColor()
        {
            // Flip flop between two colors, so every other embed has a different color.
            colorFlipFlop = !colorFlipFlop;
            return colorFlipFlop ? Color.DarkRed : Color.DarkBlue;
        }
    }
}
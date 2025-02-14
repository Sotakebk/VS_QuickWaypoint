using System.Collections.Generic;
using System.Collections.ObjectModel;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace QuickWaypoint;

internal static class EntityToWaypoint
{
    private readonly static ReadOnlyDictionary<string, VectorRgb> TraderColors = new ReadOnlyDictionary<string, VectorRgb>(new Dictionary<string, VectorRgb>()
    {
        ["humanoid-trader-foods"] = VectorRgb.FromHexRgb(0xE5F6DF),
        ["humanoid-trader-artisan"] = VectorRgb.FromHexRgb(0xFFBF00),
        ["humanoid-trader-buildmaterials"] = VectorRgb.FromHexRgb(0xE9967A),
        ["humanoid-trader-clothing"] = VectorRgb.FromHexRgb(0xAFEEEE),
        ["humanoid-trader-commodities"] = VectorRgb.FromHexRgb(0xDCDCDC),
        ["humanoid-trader-furniture"] = VectorRgb.FromHexRgb(0xDEB887),
        ["humanoid-trader-luxuries"] = VectorRgb.FromHexRgb(0xFFD700),
        ["humanoid-trader-survivalgoods"] = VectorRgb.FromHexRgb(0xF5FFFA),
        ["humanoid-trader-treasurehunter"] = VectorRgb.FromHexRgb(0xDC143C),
    });

    public static (string name, string icon, VectorRgb color) GetNameIconAndColor(EntitySelection selection)
    {
        var code = selection.Entity.Code.FirstPathPart();

        if (code.StartsWith("humanoid-trader"))
        {
            if (!TraderColors.TryGetValue(code, out var rgb))
            {
                rgb = VectorRgb.FromHexRgb(0xFFFFFF);
            }

            return (Lang.Get($"item-creature-{code}"), "trader", rgb);
        }
        if (selection.Entity.Class == "EntityTrader")
        {
            return ("Trader", "trader", VectorRgb.FromHexRgb(0xFFFFFF));
        }

        return (selection.Entity.GetName(), "circle", VectorRgb.FromHexRgb(0xFFFFFF));
    }
}

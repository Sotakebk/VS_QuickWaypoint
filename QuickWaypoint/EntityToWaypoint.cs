using Vintagestory.API.Common;

namespace QuickWaypoint;

internal static class EntityToWaypoint
{
    public static (string name, string icon, int color) GetNameIconAndColor(EntitySelection selection)
    {
        var code = selection.Entity.Code.FirstPathPart();

        if (code == "humanoid-trader-foods")
        {
            return ("Trader - Food", "trader", 0xE5F6DF);
        }
        if (code == "humanoid-trader-artisan")
        {
            return ("Trader - Artisan", "trader", 0xFFBF00);
        }
        if (code == "humanoid-trader-buildmaterials")
        {
            return ("Trader - Building Materials", "trader", 0xE9967A);
        }
        if (code == "humanoid-trader-clothing")
        {
            return ("Trader - Clothing", "trader", 0xAFEEEE);
        }
        if (code == "humanoid-trader-commodities")
        {
            return ("Trader - Commodities", "trader", 0xDCDCDC);
        }
        if (code == "humanoid-trader-furniture")
        {
            return ("Trader - Furniture", "trader", 0xDEB887);
        }
        if (code == "humanoid-trader-luxuries")
        {
            return ("Trader - Luxuries", "trader", 0xFFD700);
        }
        if (code == "trader-survivalgoods")
        {
            return ("Trader - Survival Goods", "trader", 0xF5FFFA);
        }
        if (code == "humanoid-trader-treasurehunter")
        {
            return ("Trader - Treasure Hunter", "trader", 0xDC143C);
        }
        // fallover for traders
        if (selection.Entity.Class == "EntityTrader")
        {
            return ("Trader", "trader", 0xFFFFFF);
        }

        return (selection.Entity.GetName(), "circle", 0xFFFFFF);
    }
}

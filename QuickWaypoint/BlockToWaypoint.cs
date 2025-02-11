using System;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace QuickWaypoint;

internal static class BlockToWaypoint
{
    public static (string icon, int color) GetIconAndColor(ICoreClientAPI api, BlockSelection selection)
    {
        int GetDefaultColor() => selection.Block.GetColor(api, selection.Position);

        var code = selection.Block.Code.FirstCodePart();

        if (code == "rock" || code == "loosestones" || code == "looseboulders")
        {
            return ("rocks", ConvertColorSpace(GetDefaultColor()));
        }
        if (code == "ore" || code == "looseores")
        {
            return ("pick", GetOreColor(selection.Block.Code) ?? ConvertColorSpace(Brighten(GetDefaultColor())));
        }
        if (code == "lootvessel")
        {
            return ("vessel", 0xD3D3D3);
        }
        if (code == "statictranslocator")
        {
            return ("spiral", 0x00FFFF);
        }
        if (code == "crop")
        {
            return ("turnip", ConvertColorSpace(Brighten(GetDefaultColor())));
        }
        if (code == "bigberrybush")
        {
            return ("berries", ConvertColorSpace(Brighten(GetDefaultColor())));
        }
        if (code == "fruittree")
        {
            return ("apple", ConvertColorSpace(Brighten(GetDefaultColor())));
        }
        if (code == "wildbeehive")
        {
            return ("bee", 0xFFEA00);
        }
        if (code == "log")
        {
            return ("tree2", ConvertColorSpace(Brighten(GetDefaultColor())));
        }
        if (code == "mushroom")
        {
            return ("mushroom", ConvertColorSpace(Brighten(GetDefaultColor())));
        }

        return ("circle", ConvertColorSpace(Brighten(GetDefaultColor())));
    }

    private static int Brighten(int cccc)
    {
        var c1 = (cccc >> 24) & 0xFF;
        var c2 = (cccc >> 16) & 0xFF;
        var c3 = (cccc >> 8) & 0xFF;
        var c4 = cccc & 0xFF;

        c1 = Math.Min((255 / 4) + (c1 / 4) * 3, 255);
        c2 = Math.Min((255 / 4) + (c2 / 4) * 3, 255);
        c3 = Math.Min((255 / 4) + (c3 / 4) * 3, 255);
        c4 = Math.Min((255 / 4) + (c4 / 4) * 3, 255);

        return (c1 << 24) | (c2 << 16) | (c3 << 8) | c4;
    }

    private static int ConvertColorSpace(int abgr)
    {
        var r = abgr & 0xFF;
        var b = (abgr >> 8) & 0xFF;
        var g = (abgr >> 16) & 0xFF;
        var a = (abgr >> 24) & 0xFF;

        return (a << 24) | (r << 16) | (g << 8) | b;
    }

    private static int? GetOreColor(string code)
    {
        bool ContainsAny(params string[] names) => names.Any(n => code.Contains(n, StringComparison.InvariantCultureIgnoreCase));

        if (ContainsAny("copper", "malachite"))
        {
            return 0xFFF40E;
        }
        if (ContainsAny("lead", "galena"))
        {
            return 0x6C6C6A;
        }
        if (ContainsAny("iron", "hemanite", "magnetite", "limonite"))
        {
            return 0xB8430F; // rust hehe
        }
        if (ContainsAny("gold"))
        {
            return 0xFFD700;
        }
        if (ContainsAny("silver"))
        {
            return 0xC0C0C0;
        }
        if (ContainsAny("quartz"))
        {
            return 0xFFFFFF;
        }
        if (ContainsAny("bismuth", "bismuthinite"))
        {
            return 0x879B57;
        }
        if (ContainsAny("tin", "cassiterite"))
        {
            return 0x879B57;
        }
        if (ContainsAny("zinc", "sphalerite"))
        {
            return 0x9D9B89;
        }

        return null;
    }
}

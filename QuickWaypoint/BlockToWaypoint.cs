using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace QuickWaypoint;

internal static class BlockToWaypoint
{
    public static (string icon, VectorRgb color) GetIconAndColor(ICoreClientAPI api, BlockSelection selection)
    {
        VectorRgb GetBlockColor() => GetColor(api, selection);

        var code = selection.Block.Code.FirstCodePart();

        if (code == "rock" || code == "loosestones" || code == "looseboulders")
        {
            return ("rocks", GetBlockColor());
        }
        if (code == "ore" || code == "looseores")
        {
            return ("pick", GetOreColor(selection.Block.Code) ?? GetBlockColor());
        }
        if (code == "lootvessel")
        {
            return ("vessel", VectorRgb.FromHexRgb(0xD3D3D3));
        }
        if (code == "statictranslocator")
        {
            return ("spiral", VectorRgb.FromHexRgb(0x00FFFF));
        }
        if (code == "crop")
        {
            return ("turnip", GetBlockColor());
        }
        if (code == "bigberrybush")
        {
            return ("berries", GetBlockColor());
        }
        if (code == "fruittree")
        {
            return ("apple", GetBlockColor());
        }
        if (code == "wildbeehive")
        {
            return ("bee", VectorRgb.FromHexRgb(0xFFEA00));
        }
        if (code == "log")
        {
            return ("tree2", GetBlockColor());
        }
        if (code == "mushroom")
        {
            return ("mushroom", GetBlockColor());
        }

        return ("circle", GetBlockColor());
    }

    private static VectorRgb GetColor(ICoreClientAPI api, BlockSelection selection)
    {
        var rgb = MultisampleRandomColors(api, selection, 10);
        var hsl = rgb.ToHsv();

        // brighten it a bit - scale luminance, then scale saturation
        var l = 0.5 + 0.5 * hsl.Luminance;
        double maxSaturation = (l > 0.5) ? (1 - l) / l : l / (1 - l);
        var s = hsl.Saturation * maxSaturation;
        s = (s + maxSaturation) / 2.0;
        s = Math.Min(1.0, s);

        hsl.Luminance = l;
        hsl.Saturation = s;
        return hsl.ToRgb();
    }

    private static VectorRgb MultisampleRandomColors(ICoreClientAPI api, BlockSelection selection, int iterations = 1)
    {
        double rAccumulator = 0;
        double gAccumulator = 0;
        double bAccumulator = 0;
        for (var i = 0; i < iterations; i++)
        {
            var packedRand = selection.Block.GetRandomColor(api, selection.Position, Vintagestory.API.MathTools.BlockFacing.UP);

            // why the eff is red and blue flipped - lol: https://github.com/anegostudios/vsessentialsmod/blob/master/Systems/WorldMap/ChunkLayer/ChunkMapLayer.cs#L601
            double rRand = (packedRand >> 16) & 0xFF;
            double gRand = (packedRand >> 8) & 0xFF;
            double bRand = packedRand & 0xFF;
            rAccumulator += rRand;
            gAccumulator += gRand;
            bAccumulator += bRand;
        }

        return new VectorRgb()
        {
            Red = rAccumulator / (255.0 * iterations),
            Green = gAccumulator / (255.0 * iterations),
            Blue = bAccumulator / (255.0 * iterations),
            Alpha = 1
        };
    }

    private static readonly ICollection<(string[] keywords, VectorRgb? color)> OreKeywordsToColors = new (string[], VectorRgb?)[] {
        (new[] { "copper", "malachite" }, VectorRgb.FromHexRgb(0xFFA200)),
        (new[] { "lead", "galena" }, VectorRgb.FromHexRgb(0x7483A1)),
        (new[] { "iron", "hemanite", "magnetite", "limonite" }, VectorRgb.FromHexRgb(0xB8430F)),
        (new[] { "gold" }, VectorRgb.FromHexRgb(0xFFD700)),
        (new[] { "silver" }, VectorRgb.FromHexRgb(0xC0C0C0)),
        (new[] { "quartz" }, VectorRgb.FromHexRgb(0xFFFFFF)),
        (new[] { "bismuth", "bismuthinite" }, VectorRgb.FromHexRgb(0x879B57)),
        (new[] { "tin", "cassiterite" }, VectorRgb.FromHexRgb(0xDBDBDB)),
        (new[] { "zinc", "sphalerite" }, VectorRgb.FromHexRgb(0x9D9B89)),
    };

    private static VectorRgb? GetOreColor(string code)
    {
        var (_, color) = OreKeywordsToColors.FirstOrDefault(pair => pair.keywords.Any(k => code.Contains(k)),
            defaultValue: (keywords: Array.Empty<string>(), color: null));

        return color;
    }
}

using System;

namespace QuickWaypoint;

public struct VectorRgb
{
    public double Red { get; set; }
    public double Green { get; set; }
    public double Blue { get; set; }
    public double Alpha { get; set; }

    public static VectorRgb FromRgb(byte r, byte g, byte b)
    {
        return new VectorRgb
        {
            Red = r / 255.0,
            Green = g / 255.0,
            Blue = b / 255.0,
            Alpha = 1
        };
    }

    public static VectorRgb FromHexRgb(int value)
    {
        return new VectorRgb
        {
            // #RRGGBB
            Red = ((value >> 16) & 0xff) / 255.0,
            Green = ((value >> 8) & 0xff) / 255.0,
            Blue = ((value >> 0) & 0xff) / 255.0,
        };
    }
}

public struct VectorHsv
{
    public double Hue { get; set; }
    public double Saturation { get; set; }
    public double Luminance { get; set; }
    public double Alpha { get; set; }
}

public static class ColorExtensions
{
    public static VectorHsv ToHsv(this VectorRgb color)
    {
        var (hue, saturation, luminance) = ColorMath.ConvertRgbToHsv(color.Red, color.Green, color.Blue);

        return new VectorHsv()
        {
            Alpha = color.Alpha,
            Hue = hue,
            Saturation = saturation,
            Luminance = luminance
        };
    }

    public static VectorRgb ToRgb(this VectorHsv color)
    {
        var (red, green, blue) = ColorMath.ConvertHsvToRgb(color.Hue, color.Saturation, color.Luminance);
        return new VectorRgb()
        {
            Alpha = color.Alpha,
            Red = red,
            Green = green,
            Blue = blue
        };
    }

    public static string ToArgbHexString(this VectorRgb color)
    {
        return $"{color.Alpha.ToRangeByte():X2}{color.Red.ToRangeByte():X2}{color.Green.ToRangeByte():X2}{color.Blue.ToRangeByte():X2}";
    }

    private static byte ToRangeByte(this double value)
    {
        var clamped = Math.Max(0.0, Math.Min(1.0, value));
#pragma warning disable S1244 // Floating point numbers should not be tested for equality
        return (byte)Math.Floor(clamped == 1.0 ? 255 : clamped * 256.0);
#pragma warning restore S1244
    }
}

public static class ColorMath
{
    public static (double hue, double saturation, double luminance) ConvertRgbToHsv(double red, double green, double blue)
    {
        double rNorm = red;
        double gNorm = green;
        double bNorm = blue;

        double max = Math.Max(rNorm, Math.Max(gNorm, bNorm));
        double min = Math.Min(rNorm, Math.Min(gNorm, bNorm));
        double delta = max - min;

        var luminance = (max + min) / 2.0;

#pragma warning disable S1244 // Floating point numbers should not be tested for equality
        if (delta == 0)
        {
            return (0, 0, luminance);
        }
        var saturation = (luminance > 0.5)
            ? delta / (2.0 - max - min)
            : delta / (max + min);

        double hue;
        if (max == rNorm)
        {
            hue = (gNorm - bNorm) / delta + (gNorm < bNorm ? 6 : 0);
        }
        else if (max == gNorm)
        {
            hue = (bNorm - rNorm) / delta + 2;
        }
        else
        {
            hue = (rNorm - gNorm) / delta + 4;
        }
        hue *= 60;
#pragma warning restore S1244

        return (hue, saturation, luminance);
    }

    public static (double red, double green, double blue) ConvertHsvToRgb(double hue, double saturation, double luminance)
    {
        double c = (1 - Math.Abs(2 * luminance - 1)) * saturation;
        double x = c * (1 - Math.Abs(hue / 60.0 % 2 - 1));
        double m = luminance - c / 2;

        double rPrime;
        double gPrime;
        double bPrime;

        if (0 <= hue && hue < 60)
        {
            rPrime = c; gPrime = x; bPrime = 0;
        }
        else if (60 <= hue && hue < 120)
        {
            rPrime = x; gPrime = c; bPrime = 0;
        }
        else if (120 <= hue && hue < 180)
        {
            rPrime = 0; gPrime = c; bPrime = x;
        }
        else if (180 <= hue && hue < 240)
        {
            rPrime = 0; gPrime = x; bPrime = c;
        }
        else if (240 <= hue && hue < 300)
        {
            rPrime = x; gPrime = 0; bPrime = c;
        }
        else
        {
            rPrime = c; gPrime = 0; bPrime = x;
        }

        var red = rPrime + m;
        var green = gPrime + m;
        var blue = bPrime + m;

        return (red, green, blue);
    }
}

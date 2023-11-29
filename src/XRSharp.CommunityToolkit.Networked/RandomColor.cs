using System;
using System.Windows.Media;

namespace XRSharp.CommunityToolkit.Networked;
public static class RandomColor
{
    private static readonly Random _random = new();

    public static Color Generate()
    {
        var bytes = new byte[3];
        _random.NextBytes(bytes);

        return Color.FromRgb(bytes[0], bytes[1], bytes[2]);
    }

    public static double GetRandomOpacity() => _random.NextDouble();
}

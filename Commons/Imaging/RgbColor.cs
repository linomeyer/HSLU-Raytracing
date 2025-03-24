using SixLabors.ImageSharp.PixelFormats;

namespace Commons;

public class RgbColor(double r, double g, double b)
{
    public double B { get; } = Math.Clamp(b, 0, 1);

    public double G { get; } = Math.Clamp(g, 0, 1);

    public double R { get; } = Math.Clamp(r, 0, 1);


    public static RgbColor Black => new(0, 0, 0);
    public static RgbColor Green => new(0, 1, 0);
    public static RgbColor Red => new(1, 0, 0);
    public static RgbColor White => new(1, 1, 1);
    public static RgbColor Cyan => new(0, 1, 1);
    public static RgbColor Blue => new(0, 0, 1);

    public static RgbColor operator *(RgbColor a, RgbColor b) =>
        new(
            a.R * b.R,
            a.G * b.G,
            a.B * b.B
        );

    public static RgbColor operator *(RgbColor a, double multiplier) =>
        new(
            a.R * multiplier,
            a.G * multiplier,
            a.B * multiplier
        );

    public static RgbColor operator +(RgbColor a, RgbColor b) =>
        new(
            a.R + b.R,
            a.G + b.G,
            a.B + b.B
        );

    public Rgba32 ConvertToRgba32()
    {
        var red = (byte)(R * 255 >= 255 ? 255 : R * 255);
        var green = (byte)(G * 255 >= 255 ? 255 : G * 255);
        var blue = (byte)(B * 255 >= 255 ? 255 : B * 255);
        return new Rgba32(red, green, blue, 255);
    }
}
using SixLabors.ImageSharp.PixelFormats;

namespace Commons;

public class RgbColor(double r, double g, double b)
{
    private readonly double _b = Math.Clamp(b, 0, 1);
    private readonly double _g = Math.Clamp(g, 0, 1);
    private readonly double _r = Math.Clamp(r, 0, 1);

    public double B => Math.Clamp(_b, 0, 1);
    public double G => Math.Clamp(_g, 0, 1);
    public double R => Math.Clamp(_r, 0, 1);


    public static RgbColor Orange => new(1, 0.5, 0);
    public static RgbColor Black => new(0, 0, 0);
    public static RgbColor Green => new(0, 1, 0);
    public static RgbColor Red => new(1, 0.1, 0.1);
    public static RgbColor White => new(1, 1, 1);
    public static RgbColor Cyan => new(0, 1, 0.5);
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

    protected bool Equals(RgbColor other) => _b.Equals(other._b) && _g.Equals(other._g) && _r.Equals(other._r);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((RgbColor)obj);
    }

    public override int GetHashCode() => HashCode.Combine(_b, _g, _r);
}
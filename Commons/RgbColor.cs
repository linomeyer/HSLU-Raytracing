namespace Commons;

public class RgbColor(byte r, byte g, byte b)
{
    public byte B => b;
    public byte G => g;
    public byte R => r;

    public static RgbColor Black => new(0, 0, 0);
    public static RgbColor Green => new(0, 255, 0);
    public static RgbColor Red => new(255, 0, 0);
    public static RgbColor White => new(255, 255, 255);
    public static RgbColor Cyan => new(0, 255, 255);
    public static RgbColor Blue => new(0, 0, 255);
}
using Commons.Imaging;
using SkiaSharp;

namespace SimpleCircle;

internal static class Program
{
    private const string FilePath = "center_circle.png";
    private const int Width = 800;
    private const int Height = 500;
    private const int Radius = 100;

    private static void Main()
    {
        var bitmap = GenerateBitmap();
        ImageHandler.SaveBitmapAsImage(bitmap, FilePath);
        Console.WriteLine($"Circle image saved to {FilePath}");
    }

    private static SKBitmap GenerateBitmap()
    {
        var bitmap = new SKBitmap(Width, Height);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var color = SKColors.DarkSlateGray;
                if (IsCircle(x, y)) color = SKColors.Crimson;

                bitmap.SetPixel(x, y, color);
            }

        return bitmap;
    }

    private static bool IsCircle(int x, int y)
    {
        const int centerX = Width / 2;
        const int centerY = Height / 2;

        return Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2) <= Math.Pow(Radius, 2);
    }
}
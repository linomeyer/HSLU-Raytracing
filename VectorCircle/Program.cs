using Commons;
using SkiaSharp;

namespace VectorCircle;

internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Radius = 100;
    private const string FilePath = "vector-circle.png";

    private static void Main()
    {
        var bitmap = GenerateBitmap();
        ImageHandler.SaveBitmapAsImage(bitmap, FilePath);
        Console.WriteLine($"Circle image saved to {FilePath}");
    }

    private static SKBitmap GenerateBitmap()
    {
        var circleCenter = new Vector2D(Width / 2, Height / 2);

        var bitmap = DrawCircle(circleCenter);

        return bitmap;
    }

    private static SKBitmap DrawCircle(Vector2D circleCenter)
    {
        var bitmap = new SKBitmap(Width, Height);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var color = SKColors.DarkSlateGray;
                var pixel = new Vector2D(x, y);
                if (pixel.Distance(circleCenter) <= Radius) color = SKColors.Crimson;
                bitmap.SetPixel(x, y, color);
            }

        return bitmap;
    }
}
using Commons.Imaging;
using SkiaSharp;

namespace random_pixel_image;

internal static class Program
{
    private const string FilePath = "skia_random_colors.png";
    private const int Width = 300;
    private const int Height = 300;

    private static void Main()
    {
        var bitmap = GenerateBitmap();
        ImageHandler.SaveBitmapAsImage(bitmap, FilePath);
        Console.WriteLine($"Random color image saved to {FilePath}");
    }

    private static SKBitmap GenerateBitmap()
    {
        var random = new Random();
        var bitmap = new SKBitmap(Width, Height);

        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var color = new SKColor(
                    (byte)random.Next(256), // Random Red
                    (byte)random.Next(256), // Random Green
                    (byte)random.Next(256), // Random Blue
                    255 // Full opacity
                );
                bitmap.SetPixel(x, y, color);
            }

        return bitmap;
    }

    private static void SaveBitmapAsImage(SKBitmap bitmap)
    {
        using var fs = new FileStream(FilePath, FileMode.Create);
        bitmap.Encode(fs, SKEncodedImageFormat.Png, 100);
    }
}
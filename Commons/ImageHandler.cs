using SkiaSharp;

namespace Commons;

public class ImageHandler
{
    public static void SaveBitmapAsImage(SKBitmap bitmap, string filePath)
    {
        using var fs = new FileStream(filePath, FileMode.Create);
        bitmap.Encode(fs, SKEncodedImageFormat.Png, 100);
    }
}
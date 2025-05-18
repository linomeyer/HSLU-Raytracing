using SkiaSharp;

namespace Commons.Imaging;

public static class ImageHandler
{
    public const string ImageFolderPath = "../../../../img/";

    public static void SaveBitmapAsImage(SKBitmap bitmap, string fileName)
    {
        using var fs = new FileStream(@"C:\workspace\hslu\raytracing\HSLU.Raytracing\img\" + fileName, FileMode.Create);
        Console.WriteLine("Saved image to: " + fs.Name);
        bitmap.Encode(fs, SKEncodedImageFormat.Png, 100);
    }
}
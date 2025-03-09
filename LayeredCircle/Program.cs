namespace LayeredCircle;
/*
internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const string FilePath = "layered-circle.png";

    private static void Main()
    {
        var bitmap = GenerateBitmap();
        ImageHandler.SaveBitmapAsImage(bitmap, FilePath);
        Console.WriteLine($"Circle image saved to {FilePath}");
    }

    private static SKBitmap GenerateBitmap()
    {
        var bitmap = new SKBitmap(Width, Height);

        var circle1 = new Circle(new Vector2D(400, 400), 200, new RgbColor(0f, 0f, 0.1f));
        var circle2 = new Circle(new Vector2D(300, 200), 200, new RgbColor(0.1f, 0f, 0f));
        var circle3 = new Circle(new Vector2D(500, 200), 200, new RgbColor(0f, 0.1f, 0f));

        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var pixel = new Vector2D(x, y);
                var insideCircle1 = pixel.Distance(circle1.Center) <= circle1.Radius;
                var insideCircle2 = pixel.Distance(circle2.Center) <= circle2.Radius;
                var insideCircle3 = pixel.Distance(circle3.Center) <= circle3.Radius;

                var combinedColor = new RgbColor(1, 1, 1);
                combinedColor = (insideCircle1 ? combinedColor * circle1.Color : combinedColor) *
                                (insideCircle2 ? combinedColor * circle2.Color : combinedColor) *
                                (insideCircle3 ? combinedColor * circle3.Color : combinedColor);

                bitmap.SetPixel(
                    x,
                    y,
                    new SKColor(
                        (byte)Math.Ceiling(combinedColor.R * 100),
                        (byte)Math.Ceiling(combinedColor.G * 100),
                        (byte)Math.Ceiling(combinedColor.B * 100)
                    ));
            }

        return bitmap;
    }
}
*/
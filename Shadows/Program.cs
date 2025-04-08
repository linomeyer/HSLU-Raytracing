using Commons;
using Commons._3D;
using Commons.Lighting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Shadows;

internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Depth = 600;
    private const string FilePath = ImageHandler.ImageFolderPath + "shadows.png";

    private static readonly List<IObject3D> Objects3D =
    [
        new Triangle(new Vector3D(0, 600, 600), new Vector3D(800, 600, 600), new Vector3D(0, 0, 600), RgbColor.Blue),
        new Triangle(new Vector3D(430, 540, 400), new Vector3D(500, 300, 400), new Vector3D(300, 500, 400), RgbColor.Orange),
        new Triangle(new Vector3D(598, 414, 105), new Vector3D(723, 371, 255), new Vector3D(554, 589, 192), RgbColor.Red),

        new Sphere(new Vector3D(150, 150, 200), 100, RgbColor.Green)
    ];

    private static readonly List<LightSource> LightSources =
    [
        new(
            new Vector3D(400, 0, -600),
            new RgbColor(0.5, 0.5, 0.5),
            1
        ),
        new(
            new Vector3D(0, -500, -500),
            new RgbColor(0.6, 0.6, 0.5),
            0.8
        )
    ];

    private static readonly Sphere Sphere = new(new Vector3D(650, 150, 300), 100, RgbColor.Green);

    private static void Main()
    {
        CreateImage();
        Console.WriteLine($"Planes image saved to {FilePath}");
    }

    private static void CreateImage()
    {
        var rayTracer = new RayTracer(Objects3D, LightSources);
        using var image = new Image<Rgba32>(Width, Height);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var rayIntoScreen = new Ray(new Vector3D(x, y, 0), new Vector3D(0, 0, 1));
                var color = rayTracer.CalcRay(rayIntoScreen);
                image[x, y] = color.ConvertToRgba32();
            }

        image.SaveAsPng(FilePath);
    }
}
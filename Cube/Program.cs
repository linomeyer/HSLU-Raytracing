using Commons;
using Commons._3D;
using Commons.Imaging;
using Commons.Lighting;
using Commons.Raytracer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Cube;

internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Depth = 600;
    private const string FilePath = ImageHandler.ImageFolderPath + "cube.png";

    private static readonly List<IObject3D> Objects3D =
    [
        new Triangle(new Vector3D(0, 600, 600), new Vector3D(800, 600, 600), new Vector3D(0, 0, 600), RgbColor.Blue),
        new Sphere(new Vector3D(650, 150, 300), 100, RgbColor.Green),
        new Commons._3D.Cube(new Vector3D(200, 250, 200), 150, RgbColor.Red, 30)


        /*new Triangle(new Vector3D(348, 327, 205), new Vector3D(473, 284, 355), new Vector3D(304, 502, 292), RgbColor.Red),
        new Triangle(new Vector3D(473, 284, 355), new Vector3D(429, 459, 442), new Vector3D(304, 502, 292), RgbColor.Red),
        new Triangle(new Vector3D(498, 414, 105), new Vector3D(623, 371, 255), new Vector3D(454, 589, 192), RgbColor.Red),
        new Triangle(new Vector3D(623, 371, 255), new Vector3D(579, 546, 342), new Vector3D(454, 589, 192), RgbColor.Red),
        new Triangle(new Vector3D(348, 327, 205), new Vector3D(473, 284, 355), new Vector3D(498, 414, 105), RgbColor.Red),
        new Triangle(new Vector3D(473, 284, 355), new Vector3D(623, 371, 255), new Vector3D(498, 414, 105), RgbColor.Red),
        new Triangle(new Vector3D(304, 502, 292), new Vector3D(429, 459, 442), new Vector3D(454, 589, 192), RgbColor.Red),
        new Triangle(new Vector3D(429, 459, 442), new Vector3D(579, 546, 342), new Vector3D(454, 589, 192), RgbColor.Red),
        new Triangle(new Vector3D(348, 327, 205), new Vector3D(304, 502, 292), new Vector3D(498, 414, 105), RgbColor.Red),
        new Triangle(new Vector3D(304, 502, 292), new Vector3D(454, 589, 192), new Vector3D(498, 414, 105), RgbColor.Red),
        new Triangle(new Vector3D(473, 284, 355), new Vector3D(429, 459, 442), new Vector3D(623, 371, 255), RgbColor.Red),
        new Triangle(new Vector3D(429, 459, 442), new Vector3D(579, 546, 342), new Vector3D(623, 371, 255), RgbColor.Red)*/
    ];


    private static readonly List<LightSource> LightSources =
    [
        new(new Vector3D(0, 0, -1300), new RgbColor(1, 1, 0.9), 1)
    ];


    private static void Main()
    {
        CreateImage();
        Console.WriteLine($"Planes image saved to {FilePath}");
    }

    private static void CreateImage()
    {
        var rayTracer = new Raytracer(Objects3D, LightSources);
        using var image = new Image<Rgba32>(Width, Height);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var ray = new Ray(new Vector3D(x, y, 0), new Vector3D(0, 0, 1));
                var color = rayTracer.CalcRay(ray);
                image[x, y] = color.ConvertToRgba32();
            }

        image.SaveAsPng(FilePath);
    }
}
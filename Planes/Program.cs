using Commons;
using Commons._3D;
using Commons.Lighting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Planes;

internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Depth = 600;
    private const string FilePath = ImageHandler.ImageFolderPath + "planes.png";

    private static readonly List<IObject3D> Objects3D =
    [
        new Triangle(new Vector3D(0, 600, 600), new Vector3D(800, 600, 600), new Vector3D(0, 0, 600), RgbColor.Blue),
        new Triangle(new Vector3D(430, 540, 400), new Vector3D(500, 300, 400), new Vector3D(300, 500, 400), RgbColor.Orange),
        new Triangle(new Vector3D(598, 414, 105), new Vector3D(723, 371, 255), new Vector3D(554, 589, 192), RgbColor.Red),
        new Triangle(new Vector3D(365, 300, 300), new Vector3D(235, 530, 600), new Vector3D(515, 580, 300), RgbColor.Cyan),
        new Sphere(new Vector3D(650, 150, 300), 100, RgbColor.Green)
    ];

    private static readonly List<LightSource> LightSource =
    [
        new(new Vector3D(300, 0, -250), new RgbColor(1, 1, 0.9), 1)
    ];


    private static void Main()
    {
        CreateImage();
        Console.WriteLine($"Planes image saved to {FilePath}");
    }

    private static void CreateImage()
    {
        var rayTracer = new RayTracer(Objects3D, LightSource);
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
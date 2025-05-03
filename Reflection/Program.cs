using Commons;
using Commons._3D;
using Commons.Lighting;
using Commons.Materials;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Reflection;

internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Depth = 600;
    private const string FilePath = ImageHandler.ImageFolderPath + "reflection.png";

    private static readonly List<IObject3D> Objects3D =
    [
        new Sphere(new Vector3D(150, 475, 200), 100, MaterialFactory.Create(MaterialType.Jade, 0)),
        new Cube(new Vector3D(400, 500, 200), 100, MaterialFactory.Create(MaterialType.Copper, 0.9), 30),
        new Sphere(new Vector3D(650, 475, 200), 100, MaterialFactory.Create(MaterialType.Jade, 0.9)),
        //floor
        new Triangle(new Vector3D(0, 600, 40), new Vector3D(800, 600, 40), new Vector3D(0, 500, 600), MaterialFactory.Create(MaterialType.Chrome, 0)),
        new Triangle(new Vector3D(800, 600, 40), new Vector3D(600, 500, 600), new Vector3D(0, 500, 600), MaterialFactory.Create(MaterialType.Chrome, 0))
        //walls
        //new Triangle(new Vector3D(0, 600, 40), new Vector3D(0, 0, 40), new Vector3D(40, 0, 600), MaterialFactory.Create(MaterialType.Chrome, 0)),
        //new Triangle(new Vector3D(800, 600, 40), new Vector3D(800, 0, 40), new Vector3D(760, 0, 600), MaterialFactory.Create(MaterialType.Chrome, 0))
    ];

    private static readonly List<LightSource> LightSources =
    [
        new(
            new Vector3D(400, 0, -600),
            new RgbColor(1, 1, 0.9),
            1
        ),
        new(
            new Vector3D(0, -500, -500),
            new RgbColor(1, 1, 1),
            0.8
        )
    ];

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
                var origin = new Vector3D(x, y, 0);
                var focus = new Vector3D(Width / 2, 599, -1200);
                var direction = origin - focus;
                var rayIntoScreen = new Ray(origin, direction);


                var color = rayTracer.CalcRay(rayIntoScreen);
                image[x, y] = color.ConvertToRgba32();
            }

        image.SaveAsPng(FilePath);
    }
}
using Commons;
using Commons._3D;
using Commons.Imaging;
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
    private const string FilePath = ImageHandler.ImageFolderPath + "transparency.png";

    private static readonly List<IObject3D> Objects3D =
    [
        new Sphere(new Vector3D(150, 475, 200), 100, MaterialFactory.Create(MaterialType.Gold, 0.5)),
        new Cube(new Vector3D(400, 200, 150), 200, MaterialFactory.Create(MaterialType.Emerald, 0, 0.8), 30),
        new Sphere(new Vector3D(600, 200, 500), 80, MaterialFactory.Create(MaterialType.Bronze)),
        new Sphere(new Vector3D(650, 475, 200), 100, MaterialFactory.Create(MaterialType.Gold, 0.4)),
        //floor
        new Triangle(new Vector3D(-1000, 600, 40), new Vector3D(1800, 600, 40), new Vector3D(-1000, 500, 600),
            MaterialFactory.Create(MaterialType.Obsidian, 0.3)),
        new Triangle(new Vector3D(1800, 600, 40), new Vector3D(1800, 500, 600), new Vector3D(-1000, 500, 600),
            MaterialFactory.Create(MaterialType.Obsidian, 0.3)),
        /*//walls
        //left
        new Plane(new Vector3D(-100, -300, 0), new Vector3D(200, 1000, 1000), MaterialFactory.Create(MaterialType.Turquoise)),
        //right
        new Plane(new Vector3D(900, -300, 0), new Vector3D(-200, 1000, 1000), MaterialFactory.Create(MaterialType.Brass)),
        //top
        new Triangle(new Vector3D(-100, -300, 0), new Vector3D(-100, -200, 1000), new Vector3D(900, -300, 0),
            MaterialFactory.Create(MaterialType.Chrome)),
        new Triangle(new Vector3D(900, -200, 1000), new Vector3D(-100, -200, 1000), new Vector3D(900, -300, 0),
            MaterialFactory.Create(MaterialType.Chrome)),*/

        //back
        new Triangle(new Vector3D(-200, -300, 1000), new Vector3D(-200, 600, 1000), new Vector3D(1000, -300, 1000),
            MaterialFactory.Create(MaterialType.YellowPlastic, 0.2)),
        new Triangle(new Vector3D(1000, -300, 1000), new Vector3D(-200, -300, 1000), new Vector3D(1000, 600, 1010),
            MaterialFactory.Create(MaterialType.YellowPlastic))
    ];

    private static readonly List<LightSource> LightSources =
    [
        new(
            new Vector3D(700, 400, -600),
            new RgbColor(1, 1, 0.9),
            0.9
        ),
        new(
            new Vector3D(100, 200, -700),
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
                // camera setup
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
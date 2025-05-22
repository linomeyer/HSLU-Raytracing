using Commons;
using Commons._3D;
using Commons.Lighting;
using Commons.Materials;
using Commons.Raytracer;

namespace Room;

internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Depth = 600;
    private const string FilePath = "refraction-cube-in-room.png";

    private static readonly List<IObject3D> Objects3D =
    [
        new Sphere(new Vector3D(150, 475, 200), 100, MaterialFactory.Create(MaterialType.Gold, 0.5)),
        new Cube(new Vector3D(430, 250, 150), 150, MaterialFactory.Create(MaterialType.Emerald, 0.2, 0.8), 30),
        new Sphere(new Vector3D(600, 200, 500), 80, MaterialFactory.Create(MaterialType.Bronze, 0.3)),
        new Sphere(new Vector3D(650, 475, 200), 100, MaterialFactory.Create(MaterialType.Gold, 0.4)),
        new Sphere(new Vector3D(400, 430, 150), 90, MaterialFactory.Create(MaterialType.Pearl, 0.6, 0.4)),
        //floor
        new Triangle(new Vector3D(-1000, 600, 40), new Vector3D(1800, 600, 40), new Vector3D(-1000, 500, 600),
            MaterialFactory.Create(MaterialType.Obsidian, 0.3)),
        new Triangle(new Vector3D(1800, 600, 40), new Vector3D(1800, 500, 600), new Vector3D(-1000, 500, 600),
            MaterialFactory.Create(MaterialType.Obsidian, 0.3)),
        //walls
        //left
        new Plane(new Vector3D(-100, -300, 0), new Vector3D(200, 1000, 1000), MaterialFactory.Create(MaterialType.Turquoise)),
        //right
        new Plane(new Vector3D(900, -300, 0), new Vector3D(-200, 1000, 1000), MaterialFactory.Create(MaterialType.Brass)),
        //top
        new Triangle(new Vector3D(-100, -300, 0), new Vector3D(-100, -200, 1000), new Vector3D(900, -300, 0),
            MaterialFactory.Create(MaterialType.Chrome)),
        new Triangle(new Vector3D(900, -200, 1000), new Vector3D(-100, -200, 1000), new Vector3D(900, -300, 0),
            MaterialFactory.Create(MaterialType.Chrome)),

        //back
        new Triangle(new Vector3D(-200, -300, 1000), new Vector3D(-200, 600, 1000), new Vector3D(1000, -300, 1000),
            MaterialFactory.Create(MaterialType.YellowPlastic, 0.2)),
        new Triangle(new Vector3D(1000, -300, 1000), new Vector3D(-200, 600, 1000), new Vector3D(1000, 600, 1010),
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
        var raytracer = new Raytracer(new Settings(FilePath));
        var scene = new Scene(Objects3D, LightSources, new Dimensions(800, 600));
        var camera = new Camera(new Vector3D(300, 599, -1200));

        raytracer.RenderScene(scene, camera);
    }
}
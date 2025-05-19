using Commons;
using Commons._3D;
using Commons.Imaging;
using Commons.Lighting;
using Commons.Materials;
using Commons.wavefront;

namespace Final;

internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Depth = 600;
    private const string OutputFilename = "test.png";

    private static readonly List<IObject3D> Objects3D =
    [
        new Sphere(new Vector3D(150, 475, 200), 100, MaterialFactory.Create(MaterialType.Gold, 0.5)),
        new Sphere(new Vector3D(600, 200, 500), 80, MaterialFactory.Create(MaterialType.Bronze, 0.3)),
        new Sphere(new Vector3D(650, 475, 200), 100, MaterialFactory.Create(MaterialType.Gold, 0.4)),
        // glass sphere
        new Sphere(new Vector3D(350, 350, 150), 125, MaterialFactory.Create(MaterialType.Glass, 0.25, 0.9, 1.45), true),

        //floor
        new Triangle(new Vector3D(-1000, 600, 40), new Vector3D(1800, 600, 40), new Vector3D(-1000, 500, 1000),
            MaterialFactory.Create(MaterialType.Obsidian, 0.4)),
        new Triangle(new Vector3D(1800, 600, 40), new Vector3D(1800, 500, 1000), new Vector3D(-1000, 500, 1000),
            MaterialFactory.Create(MaterialType.Obsidian, 0.4)),
        // walls
        new Plane(new Vector3D(-100, -300, 0), new Vector3D(200, 1000, 1000), MaterialFactory.Create(MaterialType.Turquoise)),
        new Plane(new Vector3D(900, -300, 0), new Vector3D(-200, 1000, 1000), MaterialFactory.Create(MaterialType.Brass, 0.05)),

        // roof
        new Triangle(new Vector3D(-100, -300, 0), new Vector3D(-100, -200, 1000), new Vector3D(900, -300, 0),
            MaterialFactory.Create(MaterialType.Chrome)),
        new Triangle(new Vector3D(900, -200, 1000), new Vector3D(-100, -200, 1000), new Vector3D(900, -300, 0),
            MaterialFactory.Create(MaterialType.Chrome)),

        new Triangle(new Vector3D(-200, -300, 1000), new Vector3D(-200, 600, 1000), new Vector3D(1000, -300, 1000),
            MaterialFactory.Create(MaterialType.Silver)),
        new Triangle(new Vector3D(1000, -300, 1000), new Vector3D(-200, 600, 1000), new Vector3D(1000, 600, 1010),
            MaterialFactory.Create(MaterialType.Silver))
    ];

    private static readonly List<LightSource> LightSources =
    [
        new(
            new Vector3D(700, 400, -600),
            new RgbColor(1, 1, 0.9),
            0.9
        ),
        new(
            new Vector3D(50, 100, -100),
            new RgbColor(1, 1, 1),
            1
        )
    ];

    private static void Main()
    {
        Objects3D.AddRange(ImportWavefront());

        var rayTracer = new RayTracer(OutputFilename);

        var scene = new Scene(Objects3D, LightSources, Width, Height);
        var camera = new Camera(new Vector3D(300, 599, -1200));

        rayTracer.RenderScene(scene, camera);

        Console.WriteLine($"Planes image saved to {ImageHandler.ImageFolderPath + OutputFilename}");
    }

    private static List<Triangle> ImportWavefront() => new WavefrontObjConverter(
        Path.GetFullPath("../../../../wavefront-export/raytracing-wavefront-text.obj"),
        MaterialFactory.Create(MaterialType.Gold, 0.4),
        new Vector3D(400, 300, 300),
        100
    ).ConvertWavefrontObjToTriangles();
}
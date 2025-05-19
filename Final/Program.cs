using Commons;
using Commons._3D;
using Commons.Imaging;
using Commons.Lighting;
using Commons.Materials;
using Commons.wavefront;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Final;

internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Depth = 600;
    private const string FilePath = ImageHandler.ImageFolderPath + "test.png";

    private static readonly List<IObject3D> Objects3D =
    [
        //new Sphere(new Vector3D(150, 475, 200), 100, MaterialFactory.Create(MaterialType.Gold, 0.5)),
        //new Cube(new Vector3D(430, 250, 150), 150, MaterialFactory.Create(MaterialType.Emerald, 0.2, 0.8), 30),
        //new Sphere(new Vector3D(600, 200, 500), 80, MaterialFactory.Create(MaterialType.Bronze, 0.3)),
        //new Sphere(new Vector3D(650, 475, 200), 100, MaterialFactory.Create(MaterialType.Gold, 0.4)),
        // glass sphere
        //new Sphere(new Vector3D(350, 350, 150), 125, MaterialFactory.Create(MaterialType.Glass, 0.25, 0.9, 1.45), true),
        //floor
        new Triangle(new Vector3D(-1000, 600, 40), new Vector3D(1800, 600, 40), new Vector3D(-1000, 500, 1000),
            MaterialFactory.Create(MaterialType.Obsidian, 0.4)),
        new Triangle(new Vector3D(1800, 600, 40), new Vector3D(1800, 500, 1000), new Vector3D(-1000, 500, 1000),
            MaterialFactory.Create(MaterialType.Obsidian, 0.4)),
        //walls
        //left
        new Plane(new Vector3D(-100, -300, 0), new Vector3D(200, 1000, 1000), MaterialFactory.Create(MaterialType.Turquoise)),
        //right
        new Plane(new Vector3D(900, -300, 0), new Vector3D(-200, 1000, 1000), MaterialFactory.Create(MaterialType.Brass, 0.05)),
        //top
        new Triangle(new Vector3D(-100, -300, 0), new Vector3D(-100, -200, 1000), new Vector3D(900, -300, 0),
            MaterialFactory.Create(MaterialType.Chrome)),
        new Triangle(new Vector3D(900, -200, 1000), new Vector3D(-100, -200, 1000), new Vector3D(900, -300, 0),
            MaterialFactory.Create(MaterialType.Chrome)),

        //back
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
        var objTriangles = new WavefrontObjConverter(
            Path.GetFullPath("../../../../wavefront-export/raytracing-wavefront-text.obj"),
            MaterialFactory.Create(MaterialType.Gold, 0.4),
            new Vector3D(400, 300, 300),
            100
        ).ConvertWavefrontObjToTriangles();
        Objects3D.AddRange(objTriangles);

        CreateImage();
        Console.WriteLine($"Planes image saved to {FilePath}");
    }

    private static void CreateImage()
    {
        var taskLineRanges = CalcTaskLineRanges();
        var tasks = new List<Task>();
        var objLock = new object();

        var rayTracer = new RayTracer(Objects3D, LightSources);
        using var image = new Image<Rgba32>(Width, Height);
        var camera = new Camera(new Vector3D(300, 599, -1200));

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                // camera setup
                var origin = new Vector3D(x, y, 0);
                var ray = camera.CreateRay(origin);

                var color = rayTracer.CalcRay(ray);
                image[x, y] = color.ConvertToRgba32();
            }

            Console.WriteLine("Line rendered: " + y + " / " + Height);
        }

/*
        foreach (var taskRange in taskLineRanges)
        {
            var task = Task.Run(() =>
            {
                foreach (var y in taskRange)
                    for (var x = 0; x < Width; x++)
                    {
                        var ray = camera.CreateRay(new Vector3D(x, y, 0));

                        var color = rayTracer.CalcRay(ray);
                        lock (objLock)
                        {
                            image[x, y] = color.ConvertToRgba32();
                        }
                    }
            });
            tasks.Add(task);
        }

        Task.WaitAll(tasks);
*/
        image.SaveAsPng(FilePath);
    }

    private static List<List<int>> CalcTaskLineRanges()
    {
        var randomizedLines = Enumerable.Range(0, Height - 1).OrderBy(_ => Guid.NewGuid()).ToList();
        return randomizedLines
            .Select((item, index) => new { item, index })
            .GroupBy(x => x.index / (randomizedLines.Count / 12))
            .Select(group => group.Select(x => x.item).ToList())
            .ToList();
    }
}
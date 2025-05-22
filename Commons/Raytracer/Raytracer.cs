using Commons._3D;
using Commons.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Commons.Raytracer;

public class Raytracer(Settings settings)
{
    public void RenderScene(Scene scene, Camera camera)
    {
        var rayCalculator = new RayCalculator(scene, settings.Depth);

        if (settings.DoMultithreading) RenderMulti(scene, camera, rayCalculator);
        else Render(scene, camera, rayCalculator);
    }

    private void Render(Scene scene, Camera camera, RayCalculator rayCalculator)
    {
        using var image = new Image<Rgba32>(scene.Width, scene.Height);
        for (var y = 0; y < scene.Height; y++)
        {
            for (var x = 0; x < scene.Width; x++)
            {
                // camera setup
                var origin = new Vector3D(x, y, 0);
                var ray = camera.CreateRay(origin);

                var color = rayCalculator.Calc(ray);
                image[x, y] = color.ConvertToRgba32();
            }

            Console.WriteLine("Line rendered: " + y + " / " + scene.Height);
        }

        image.SaveAsPng(ImageHandler.ImageFolderPath + settings.OutputFilename);
    }

    private void RenderMulti(Scene scene, Camera camera, RayCalculator rayCalculator)
    {
        using var image = new Image<Rgba32>(scene.Width, scene.Height);

        var taskLineRanges = CalcTaskLineRanges(scene.Height);
        var tasks = new List<Task>();
        var objLock = new object();

        foreach (var taskRange in taskLineRanges)
        {
            var task = Task.Run(() =>
            {
                foreach (var y in taskRange)
                    for (var x = 0; x < scene.Width; x++)
                    {
                        var ray = camera.CreateRay(new Vector3D(x, y, 0));

                        var color = rayCalculator.Calc(ray);
                        lock (objLock)
                        {
                            image[x, y] = color.ConvertToRgba32();
                        }
                    }
            });
            tasks.Add(task);
        }

        Task.WaitAll(tasks);

        image.SaveAsPng(ImageHandler.ImageFolderPath + settings.OutputFilename);
    }

    private static List<List<int>> CalcTaskLineRanges(int height)
    {
        var randomizedLines = Enumerable.Range(0, height - 1).OrderBy(_ => Guid.NewGuid()).ToList();
        return randomizedLines
            .Select((item, index) => new { item, index })
            .GroupBy(x => x.index / (randomizedLines.Count / 12))
            .Select(group => group.Select(x => x.item).ToList())
            .ToList();
    }
}
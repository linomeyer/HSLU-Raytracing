using Commons;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Diffusion;

internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Depth = 600;
    private const string FilePath = "spheres.png";

    private const int CenterWidth = Width / 2;
    private const int CenterHeight = Height / 2;
    private const int CenterDepth = Depth / 2;

    private static readonly List<Sphere> Spheres =
    [
        new(
            new Vector3D(CenterWidth, CenterHeight, CenterDepth + 200),
            250,
            RgbColor.Green)
    ];

    private static void Main()
    {
        CreateImage();
        Console.WriteLine($"Circle image saved to {FilePath}");
    }

    private static void CreateImage()
    {
        using var image = new Image<Rgba32>(Width, Height);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var color = Color.Black;
                var distanceToScreen = double.MaxValue;

                var currentPos = new Vector3D(x, y, 0);
                var ray = new Vector3D(0, 0, 1);
                foreach (var sphere in Spheres.OrderByDescending(s => s.Center.Z))
                {
                    var intersectionDistance = sphere.IntersectionDistance(currentPos, ray);

                    var hittingPoint = currentPos + ray * (int)intersectionDistance;
                    var n = (hittingPoint - sphere.Center).Normalize();
                    var s = (ray - hittingPoint).Normalize();

                    var colorFactor = s.ScalarProduct(n) > 0 ? s.ScalarProduct(n) : 0;

                    if (distanceToScreen > 0 && intersectionDistance < distanceToScreen)
                    {
                        distanceToScreen = intersectionDistance;
                        var brightness = (byte)(255 * colorFactor);
                        color = new Rgba32(sphere.Color.R, sphere.Color.G, sphere.Color.B, brightness);
                    }
                }

                image[x, y] = color;
            }

        image.SaveAsPng(FilePath);
    }
}
using Commons;
using Commons.Lighting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Depth = 600;
    private const string FilePath = "cube.png";

    private const int CenterWidth = Width / 2;
    private const int CenterHeight = Height / 2;
    private const int CenterDepth = Depth / 2;

    private static readonly List<Plane> Planes =
    [
        new(
            new Vector3D(348, 327, 205),
            new Vector3D(473, 284, 355),
            new Vector3D(304, 502, 292),
            RgbColor.Green
        ),
        new(new Vector3D(473, 284, 355), new Vector3D(429, 459, 442), new Vector3D(304, 502, 292), RgbColor.Blue),
        new(new Vector3D(498, 414, 105), new Vector3D(623, 371, 255), new Vector3D(454, 589, 192), RgbColor.Red)
    ];

    private static readonly LightSource LightSource =
        new(
            new Vector3D(500, 0, -10),
            new RgbColor(1, 1, 0.9),
            1
        );

    private static void Main()
    {
        CreateImage();
        Console.WriteLine($"Cube image saved to {FilePath}");
    }

    private static void CreateImage()
    {
        using var image = new Image<Rgba32>(Width, Height);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var nearestLambda = double.MaxValue;
                var color = new RgbColor(0, 0, 0);

                var ray = new Ray(new Vector3D(x, y, 0), new Vector3D(0, 0, 1));

                foreach (var plane in Planes)
                {
                    var currentLambda = plane.Lambda(ray);
                    if (plane.RayIntersectsPlane(ray))
                        if (currentLambda < nearestLambda)
                        {
                            nearestLambda = currentLambda;

                            var intersectionPoint = plane.IntersectionPoint(ray);
                            var vectorToLightSource = (LightSource.Position - intersectionPoint).Normalize();
                            var scalarProductOfNormalizedPLaneToLightSource =
                                Math.Max(0, plane.NormalVector.ScalarProduct(vectorToLightSource));

                            color = plane.Color * LightSource.Color *
                                    scalarProductOfNormalizedPLaneToLightSource * LightSource.Intensity
                                    + new RgbColor(0.1, 0.1, 0.1);
                        }
                }

                image[x, y] = color.ConvertToRgba32();
            }

        image.SaveAsPng(FilePath);
    }
}
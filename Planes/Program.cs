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

    private static readonly List<Plane> Planes =
    [
        new(new Vector3D(0, 600, 600), new Vector3D(800, 600, 600), new Vector3D(0, 0, 600), RgbColor.Blue),

        new(new Vector3D(430, 540, 400), new Vector3D(500, 300, 400), new Vector3D(300, 500, 400), RgbColor.Orange),

        new(new Vector3D(598, 414, 105), new Vector3D(723, 371, 255), new Vector3D(554, 589, 192), RgbColor.Red),

        new(new Vector3D(365, 300, 300), new Vector3D(235, 530, 600), new Vector3D(515, 580, 300), RgbColor.Cyan)
    ];

    private static readonly LightSource LightSource =
        new(
            new Vector3D(300, 0, -250),
            new RgbColor(1, 1, 0.9),
            1
        );

    private static readonly Sphere Sphere = new(new Vector3D(650, 150, 300), 100, RgbColor.Green);

    private static void Main()
    {
        CreateImage();
        Console.WriteLine($"Planes image saved to {FilePath}");
    }

    private static void CreateImage()
    {
        using var image = new Image<Rgba32>(Width, Height);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var nearestLambda = double.MaxValue;
                var color = RgbColor.Black;

                var ray = new Ray(new Vector3D(x, y, 0), new Vector3D(0, 0, 1));

                foreach (var plane in Planes)
                {
                    var currentLambda = plane.NextIntersection(ray);
                    if (currentLambda < nearestLambda)
                    {
                        nearestLambda = currentLambda;
                        color = ColorPlane(ray, plane);
                    }
                }

                var sphereColor = ColorSphere(ray);
                if (!sphereColor.Equals(RgbColor.Black))
                    color = sphereColor;

                image[x, y] = color.ConvertToRgba32();
            }

        image.SaveAsPng(FilePath);
    }

    private static RgbColor ColorPlane(Ray ray, Plane plane)
    {
        var intersectionPoint = plane.IntersectionPoint(ray);
        var vectorToLightSource = (LightSource.Position - intersectionPoint).Normalize();
        var scalarProductOfNormalizedPLaneToLightSource =
            Math.Max(0, plane.NormalVector.ScalarProduct(vectorToLightSource));

        return plane.Color * LightSource.Color *
               scalarProductOfNormalizedPLaneToLightSource * LightSource.Intensity
               + new RgbColor(0.1, 0.1, 0.1);
    }

    private static RgbColor ColorSphere(Ray ray)
    {
        var intersectionDistance = Sphere.IntersectionDistance(ray.Origin, ray.Direction);

        var hittingPoint = ray.Origin + ray.Direction * intersectionDistance;
        var n = (hittingPoint - Sphere.Center).Normalize();
        var s = (LightSource.Position - hittingPoint).Normalize();

        var colorFactor = s.ScalarProduct(n) > 0 ? s.ScalarProduct(n) : 0;

        if (intersectionDistance < double.MaxValue)
            return new RgbColor(Sphere.Color.R, Sphere.Color.G, Sphere.Color.B) * colorFactor +
                   new RgbColor(0.1, 0.1, 0.1);

        return RgbColor.Black;
    }
}
using Commons;
using Commons._3D;
using Commons.Lighting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Shadows;

internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Depth = 600;
    private const string FilePath = ImageHandler.ImageFolderPath + "shadows.png";

    private static readonly List<Triangle> Planes =
    [
        new(new Vector3D(0, 600, 600), new Vector3D(800, 600, 600), new Vector3D(0, 0, 600), RgbColor.Blue),

        new(new Vector3D(430, 540, 400), new Vector3D(500, 300, 400), new Vector3D(300, 500, 400), RgbColor.Orange),

        new(new Vector3D(598, 414, 105), new Vector3D(723, 371, 255), new Vector3D(554, 589, 192), RgbColor.Red)
    ];

    private static readonly List<LightSource> LightSources =
    [
        new(
            new Vector3D(400, 0, -250),
            new RgbColor(0.5, 0.5, 0.5),
            1
        ),
        new(
            new Vector3D(0, -500, -500),
            new RgbColor(0.6, 0.6, 0.5),
            0.8
        )
    ];

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
                var color = RgbColor.Black;
                var nearestLambda = double.MaxValue;

                var rayIntoScreen = new Ray(new Vector3D(x, y, 0), new Vector3D(0, 0, 1));

                foreach (var plane in Planes)
                {
                    var currentLambda = plane.NextIntersection(rayIntoScreen);
                    if (currentLambda < nearestLambda)
                    {
                        color = RgbColor.Black;
                        nearestLambda = currentLambda;
                        var intersectionPoint = rayIntoScreen.Origin + rayIntoScreen.Direction * currentLambda;

                        foreach (var lightSource in LightSources)
                        {
                            var intersectionPointIsInShadow = false;
                            var vectorToLightSource = (lightSource.Position - intersectionPoint).Normalize();
                            var rayToLightSource = new Ray(intersectionPoint + vectorToLightSource * MathConstants.Epsilon, vectorToLightSource);

                            intersectionPointIsInShadow = CheckRayIntersectionWithOtherObjects(rayToLightSource, intersectionPoint, lightSource);

                            if (!intersectionPointIsInShadow)
                            {
                                var scalarProductOfNormalizedPLaneToLightSource = Math.Max(0, plane.Normalized.ScalarProduct(vectorToLightSource));
                                color += plane.Color * lightSource.Color * scalarProductOfNormalizedPLaneToLightSource * lightSource.Intensity;
                            }
                        }

                        color += plane.Color * new RgbColor(0.1, 0.1, 0.1);
                    }
                }

                image[x, y] = color.ConvertToRgba32();
            }

        image.SaveAsPng(FilePath);
    }

    private static bool CheckRayIntersectionWithOtherObjects(Ray rayToLightSource, Vector3D intersectionPoint, LightSource lightSource)
    {
        var intersectionPointIsInShadow = false;
        var distanceToLightSource = Math.Abs((intersectionPoint - lightSource.Position).Length);
        foreach (var plane2 in Planes)
        {
            var lambda2 = plane2.NextIntersection(rayToLightSource);
            var intersectionPointWithOtherObject = rayToLightSource.Origin + rayToLightSource.Direction * lambda2;

            var distanceToIntersectionPointWithOtherObject =
                Math.Abs((intersectionPoint - intersectionPointWithOtherObject).Length);

            if (distanceToIntersectionPointWithOtherObject < distanceToLightSource)
                intersectionPointIsInShadow = true;
        }

        return intersectionPointIsInShadow;
    }
}
﻿using Commons;
using Commons._3D;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace LotsOfSpheres;

internal static class Program
{
    private const int Width = 800;
    private const int Height = 600;
    private const int Depth = 600;
    private const string FilePath = "spheres.png";

    private static void Main()
    {
        var spheres = GenerateSpheres();
        CreateImage(spheres);
        Console.WriteLine($"Circle image saved to {FilePath}");
    }

    private static List<Sphere> GenerateSpheres()
    {
        var spheres = new List<Sphere>();
        for (var i = 0; i < 200; i++)
        {
            var random = new Random();
            var x = random.Next(0, Width);
            var y = random.Next(0, Height);
            var z = random.Next(0, Depth);
            var radius = random.Next(20, 60);
            var color = new RgbColor((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));

            spheres.Add(new Sphere(new Vector3D(x, y, z), radius, color));
        }

        return spheres;
    }

    private static void CreateImage(List<Sphere> spheres)
    {
        using var image = new Image<Rgba32>(Width, Height);

        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var color = Color.Black;
                var distanceToScreen = double.MaxValue;
                var ray = new Ray(new Vector3D(x, y, 0), new Vector3D(0, 0, 1));

                foreach (var sphere in spheres)
                {
                    var (_, intersectionDistance) = sphere.NextIntersection(ray);
                    if (distanceToScreen > 0 && intersectionDistance < distanceToScreen)
                    {
                        distanceToScreen = intersectionDistance;
                        var red = (byte)(sphere.Color.R * 255 >= 255 ? 255 : sphere.Color.R * 255);
                        var green = (byte)(sphere.Color.G * 255 >= 255 ? 255 : sphere.Color.G * 255);
                        var blue = (byte)(sphere.Color.B * 255 >= 255 ? 255 : sphere.Color.B * 255);
                        var alpha = (byte)(255 * (1 - intersectionDistance / Depth));
                        color = new Rgba32(red, green, blue, alpha);
                    }
                }

                image[x, y] = color;
            }

        image.SaveAsPng(FilePath);
    }
}
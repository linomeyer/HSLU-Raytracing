﻿using Commons;
using Commons._3D;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SphereCutter;

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
            new Vector3D(CenterWidth, CenterHeight, CenterDepth),
            200,
            RgbColor.Blue), // Base blue sphere
        new(
            new Vector3D(CenterWidth - 70, CenterHeight - 90, CenterDepth - 70),
            100,
            new RgbColor(0, 255, 255)), // Left cyan sphere
        new(
            new Vector3D(CenterWidth + 70, CenterHeight - 90, CenterDepth - 70),
            100,
            new RgbColor(0, 255, 255)), // Right cyan sphere
        new(
            new Vector3D(CenterWidth - 110, CenterHeight - 130, CenterDepth - 200),
            40,
            new RgbColor(0, 255, 0)), // Left green sphere
        new(
            new Vector3D(CenterWidth + 110, CenterHeight - 130, CenterDepth - 200),
            40,
            new RgbColor(0, 255, 0)), // Right green sphere
        new(
            new Vector3D(CenterWidth, CenterHeight, CenterDepth - 250),
            40,
            RgbColor.Red) // Center red sphere
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

                var ray = new Ray(new Vector3D(x, y, 0), new Vector3D(0, 0, 1));
                foreach (var sphere in Spheres.OrderByDescending(s => s.Center.Z))
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
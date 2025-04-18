﻿namespace Commons.Materials;

public class MaterialFactory
{
    public static Material Create(MaterialType type, double reflectivity)
    {
        return type switch
        {
            MaterialType.Emerald => new Material(type, new RgbColor(0.0215, 0.1745, 0.0215), new RgbColor(0.07568, 0.61424, 0.07568),
                new RgbColor(0.633, 0.727811, 0.633), 0.6, reflectivity),

            MaterialType.Jade => new Material(type, new RgbColor(0.135, 0.2225, 0.1575), new RgbColor(0.54, 0.89, 0.63),
                new RgbColor(0.316228, 0.316228, 0.316228), 0.1, reflectivity),

            MaterialType.Obsidian => new Material(type, new RgbColor(0.05375, 0.05, 0.06625), new RgbColor(0.18275, 0.17, 0.22525),
                new RgbColor(0.332741, 0.328634, 0.346435), 0.3, reflectivity),

            MaterialType.Pearl => new Material(type, new RgbColor(0.25, 0.20725, 0.20725), new RgbColor(1.0, 0.829, 0.829),
                new RgbColor(0.296648, 0.296648, 0.296648), 0.088, reflectivity),

            MaterialType.Ruby => new Material(type, new RgbColor(0.1745, 0.01175, 0.01175), new RgbColor(0.61424, 0.04136, 0.04136),
                new RgbColor(0.727811, 0.626959, 0.626959), 0.6, reflectivity),

            MaterialType.Turquoise => new Material(type, new RgbColor(0.1, 0.18725, 0.1745), new RgbColor(0.396, 0.74151, 0.69102),
                new RgbColor(0.297254, 0.30829, 0.306678), 0.1, reflectivity),

            MaterialType.Brass => new Material(type, new RgbColor(0.329412, 0.223529, 0.027451), new RgbColor(0.780392, 0.568627, 0.113725),
                new RgbColor(0.992157, 0.941176, 0.807843), 0.21794872, reflectivity),

            MaterialType.Bronze => new Material(type, new RgbColor(0.2125, 0.1275, 0.054), new RgbColor(0.714, 0.4284, 0.18144),
                new RgbColor(0.393548, 0.271906, 0.166721), 0.2, reflectivity),

            MaterialType.Chrome => new Material(type, new RgbColor(0.25, 0.25, 0.25), new RgbColor(0.4, 0.4, 0.4), new RgbColor(0.774597, 0.774597, 0.774597),
                0.6, reflectivity),

            MaterialType.Copper => new Material(type, new RgbColor(0.19125, 0.0735, 0.0225), new RgbColor(0.7038, 0.27048, 0.0828),
                new RgbColor(0.256777, 0.137622, 0.086014), 0.1, reflectivity),

            MaterialType.Gold => new Material(type, new RgbColor(0.24725, 0.1995, 0.0745), new RgbColor(0.75164, 0.60648, 0.22648),
                new RgbColor(0.628281, 0.555802, 0.366065), 0.4, reflectivity),

            MaterialType.Silver => new Material(type, new RgbColor(0.19225, 0.19225, 0.19225), new RgbColor(0.50754, 0.50754, 0.50754),
                new RgbColor(0.508273, 0.508273, 0.508273), 0.4, reflectivity),

            MaterialType.BlackPlastic => new Material(type, new RgbColor(0.0, 0.0, 0.0), new RgbColor(0.01, 0.01, 0.01), new RgbColor(0.5, 0.5, 0.5), 0.25,
                reflectivity),

            MaterialType.CyanPlastic => new Material(type, new RgbColor(0.0, 0.1, 0.06), new RgbColor(0.0, 0.50980392, 0.50980392),
                new RgbColor(0.50196078, 0.50196078, 0.50196078), 0.25, reflectivity),

            MaterialType.GreenPlastic => new Material(type, new RgbColor(0.0, 0.0, 0.0), new RgbColor(0.1, 0.35, 0.1), new RgbColor(0.45, 0.55, 0.45), 0.25,
                reflectivity),
            MaterialType.RedPlastic => new Material(type, new RgbColor(0.0, 0.0, 0.0), new RgbColor(0.5, 0.0, 0.0), new RgbColor(0.7, 0.6, 0.6), 0.25,
                reflectivity),
            MaterialType.WhitePlastic => new Material(type, new RgbColor(0.0, 0.0, 0.0), new RgbColor(0.55, 0.55, 0.55), new RgbColor(0.7, 0.7, 0.7), 0.25,
                reflectivity),
            MaterialType.YellowPlastic => new Material(type, new RgbColor(0.0, 0.0, 0.0), new RgbColor(0.5, 0.5, 0.0), new RgbColor(0.6, 0.6, 0.5), 0.25,
                reflectivity),

            MaterialType.BlackRubber => new Material(type, new RgbColor(0.02, 0.02, 0.02), new RgbColor(0.01, 0.01, 0.01), new RgbColor(0.4, 0.4, 0.4),
                0.078125, reflectivity),

            MaterialType.CyanRubber => new Material(type, new RgbColor(0.0, 0.05, 0.05), new RgbColor(0.4, 0.5, 0.5), new RgbColor(0.04, 0.7, 0.7), 0.078125,
                reflectivity),
            MaterialType.GreenRubber => new Material(type, new RgbColor(0.0, 0.05, 0.0), new RgbColor(0.4, 0.5, 0.4), new RgbColor(0.04, 0.7, 0.04), 0.078125,
                reflectivity),
            MaterialType.RedRubber => new Material(type, new RgbColor(0.05, 0.0, 0.0), new RgbColor(0.5, 0.4, 0.4), new RgbColor(0.7, 0.04, 0.04), 0.078125,
                reflectivity),
            MaterialType.WhiteRubber => new Material(type, new RgbColor(0.05, 0.05, 0.05), new RgbColor(0.5, 0.5, 0.5), new RgbColor(0.7, 0.7, 0.7), 0.078125,
                reflectivity),
            MaterialType.YellowRubber => new Material(type, new RgbColor(0.05, 0.05, 0.0), new RgbColor(0.5, 0.5, 0.4), new RgbColor(0.7, 0.7, 0.04), 0.078125,
                reflectivity),

            _ => throw new ArgumentException("Unknown Material Type")
        };
    }
}
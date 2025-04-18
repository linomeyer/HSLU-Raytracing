﻿namespace Commons.Materials;

public class Material(MaterialType type, RgbColor ambient, RgbColor diffuse, RgbColor specular, double shininess)
{
    public MaterialType Name => type;
    public RgbColor Ambient => ambient;
    public RgbColor Diffuse => diffuse;
    public RgbColor Specular => specular;
    public double Shininess => shininess;
}
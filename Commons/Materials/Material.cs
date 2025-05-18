namespace Commons.Materials;

public class Material(
    MaterialType type,
    RgbColor ambient,
    RgbColor diffuse,
    RgbColor specular,
    double shininess,
    double reflectivity = 0,
    double transparency = 0,
    double refractiveIndex = 0)
{
    public MaterialType Name => type;
    public RgbColor Ambient => ambient;
    public RgbColor Diffuse => diffuse;
    public RgbColor Specular => specular;
    public double Shininess => shininess;
    public double Reflectivity => reflectivity;
    public double Transparency => transparency;
    public double RefractiveIndex => refractiveIndex;
}
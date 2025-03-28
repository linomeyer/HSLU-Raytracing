using Commons._3D;

namespace Commons.Lighting;

public class LightSource(Vector3D position, RgbColor color, double intensity)
{
    public Vector3D Position => position;
    public RgbColor Color => color;
    public double Intensity => intensity;
}
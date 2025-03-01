namespace Commons;

public class Sphere(Vector3D center, int radius, RgbColor color)
{
    public Vector3D Center => center;
    public int Radius => radius;
    public RgbColor Color => color;

    public bool IsInSphere(Vector2D point)
    {
        var dx = point.X - Center.X;
        var dy = point.Y - Center.Y;
        return dx * dx + dy * dy <= Math.Pow(Radius, 2);
    }
}
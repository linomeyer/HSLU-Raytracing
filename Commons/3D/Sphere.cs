namespace Commons._3D;

public class Sphere(Vector3D center, int radius, RgbColor color)
{
    public Vector3D Center => center;
    public int Radius => radius;
    public RgbColor Color => color;

    public double IntersectionDistance(Vector3D position, Vector3D ray)
    {
        var u = ray;
        var v = position - Center;

        var a = u.ScalarProduct(u);
        var b = 2 * u.ScalarProduct(v);
        var c = v.ScalarProduct(v) - Math.Pow(radius, 2);

        // Formula = b^2 - 4ac
        var discriminant = Math.Pow(b, 2) - 4 * a * c;

        if (discriminant < 0) return double.MaxValue;

        var x1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
        var x2 = (-b - Math.Sqrt(discriminant)) / (2 * a);

        return x1 < x2 ? x1 : x2;
    }
}
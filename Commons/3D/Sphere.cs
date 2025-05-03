using Commons.Materials;

namespace Commons._3D;

public class Sphere(Vector3D center, int radius, Material material) : IObject3D
{
    public Vector3D Center => center;
    public int Radius => radius;
    public Material Material => material;
    public Vector3D Normalized { get; set; } = new(0, 0, 0);

    public (bool hasHit, double intersectionDistance) NextIntersection(Ray ray)
    {
        var u = ray.Direction;
        var v = ray.Origin - Center;

        var a = u.ScalarProduct(u);
        var b = 2 * u.ScalarProduct(v);
        var c = v.ScalarProduct(v) - Math.Pow(radius, 2);

        // Formula = b^2 - 4ac
        var discriminant = Math.Pow(b, 2) - 4 * a * c;

        if (discriminant < 0) return (false, double.MaxValue);

        var x1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
        var x2 = (-b - Math.Sqrt(discriminant)) / (2 * a);


        var intersectionDistance = x1 < x2 ? x1 : x2;

        // workaround for catching self intersection
        if (intersectionDistance < MathConstants.Epsilon) return (false, double.MaxValue);

        Normalized = (ray.Origin + ray.Direction * intersectionDistance - Center).Normalize();

        return (true, intersectionDistance);
    }
}
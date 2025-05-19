using Commons.Materials;

namespace Commons._3D;

public class Triangle : IObject3D
{
    public Triangle(Vector3D a, Vector3D b, Vector3D c, Material material)
    {
        A = a;
        B = b;
        C = c;
        Material = material;

        var v = b - a;
        var w = c - a;
        Normalized = v.CrossProduct(w).Normalize();
    }

    public Vector3D A { get; }
    public Vector3D B { get; }
    public Vector3D C { get; }
    public Material Material { get; }
    public Vector3D Normalized { get; set; }

    /**
     * seems faster and works more accurately more accurate
     */
    public (bool hasHit, double intersectionDistance) NextIntersection(Ray ray)
    {
        var v = B - A;
        var w = C - A;
        var h = ray.Direction.CrossProduct(w);
        var a = v.ScalarProduct(h);

        if (Math.Abs(a) < MathConstants.Epsilon) return (false, double.MaxValue);

        var f = 1 / a;
        var s = ray.Origin - A;
        var u = f * s.ScalarProduct(h);
        if (u < 0 || u > 1) return (false, double.MaxValue);

        var q = s.CrossProduct(v);
        var v2 = f * ray.Direction.ScalarProduct(q);
        if (v2 < 0 || u + v2 > 1) return (false, double.MaxValue);

        var x = f * w.ScalarProduct(q);
        return x > MathConstants.Epsilon ? (true, x) : (false, double.MaxValue);
    }

    /**
     * @Deprecated
     */
    public (bool hasHit, double intersectionDistance) NextIntersection2(Ray ray)
    {
        var lambda = Lambda(ray);
        if (lambda > 0)
        {
            var q = ray.Origin + ray.Direction * lambda; // point of intersection

            var aq = q - A;
            var bq = q - B;
            var cq = q - C;

            var ba = A - B;
            var cb = B - C;
            var ac = C - A;

            var v1 = bq.CrossProduct(ba);
            var v2 = cq.CrossProduct(cb);
            var v3 = aq.CrossProduct(ac);

            if (CheckEqualPrefix(v1.Z, v2.Z, v3.Z)) return (true, lambda);
        }

        return (false, double.MaxValue);
    }

    /**
     * @Deprecated
     */
    private double Lambda(Ray ray)
    {
        var p = ray.Origin;
        var u = ray.Direction;

        var denominator = u.ScalarProduct(Normalized);
        if (Math.Abs(denominator) < MathConstants.Epsilon) return double.MaxValue;

        var lambda = (A - p).ScalarProduct(Normalized) / denominator;
        return lambda > 0 ? lambda : double.MaxValue;
    }

    /**
     * @Deprecated
     */
    private bool CheckEqualPrefix(double v1Z, double v2Z, double v3Z) =>
        (v1Z >= 0 && v2Z >= 0 && v3Z >= 0)
        ||
        (v1Z < 0 && v2Z < 0 && v3Z < 0);
}
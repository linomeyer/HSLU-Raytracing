using Commons._3D;

namespace Commons.BVH;

public class AABB
{
    public AABB(Vector3D min, Vector3D max)
    {
        Min = min;
        Max = max;
    }

    public Vector3D Min { get; }
    public Vector3D Max { get; }

    public bool Hit(Ray ray)
    {
        var tMin = double.MinValue;
        var tMax = double.MaxValue;

        // Test X axis
        var invD = 1.0 / ray.Direction.X;
        var t0 = (Min.X - ray.Origin.X) * invD;
        var t1 = (Max.X - ray.Origin.X) * invD;

        if (invD < 0.0) (t0, t1) = (t1, t0);

        tMin = Math.Max(t0, tMin);
        tMax = Math.Min(t1, tMax);

        if (tMax <= tMin)
            return false;

        // Test Y axis
        invD = 1.0 / ray.Direction.Y;
        t0 = (Min.Y - ray.Origin.Y) * invD;
        t1 = (Max.Y - ray.Origin.Y) * invD;

        if (invD < 0.0) (t0, t1) = (t1, t0);

        tMin = Math.Max(t0, tMin);
        tMax = Math.Min(t1, tMax);

        if (tMax <= tMin)
            return false;

        // Test Z axis
        invD = 1.0 / ray.Direction.Z;
        t0 = (Min.Z - ray.Origin.Z) * invD;
        t1 = (Max.Z - ray.Origin.Z) * invD;

        if (invD < 0.0) (t0, t1) = (t1, t0);

        tMin = Math.Max(t0, tMin);
        tMax = Math.Min(t1, tMax);

        return tMax > tMin;
    }

    public static AABB SurroundingBox(AABB box1, AABB box2)
    {
        var small = new Vector3D(
            Math.Min(box1.Min.X, box2.Min.X),
            Math.Min(box1.Min.Y, box2.Min.Y),
            Math.Min(box1.Min.Z, box2.Min.Z)
        );

        var big = new Vector3D(
            Math.Max(box1.Max.X, box2.Max.X),
            Math.Max(box1.Max.Y, box2.Max.Y),
            Math.Max(box1.Max.Z, box2.Max.Z)
        );

        return new AABB(small, big);
    }
}
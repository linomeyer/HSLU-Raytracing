using Commons.Materials;

namespace Commons._3D;

public class Plane : IObject3D
{
    private readonly (Triangle left, Triangle right) _triangles;

    public Plane(Vector3D origin, Vector3D direction, Material material)
    {
        Material = material;

        var a = origin;
        var b = new Vector3D(origin.X + direction.X, origin.Y, origin.Z + direction.Z);
        var c = new Vector3D(origin.X, origin.Y + direction.Y, origin.Z);
        var d = origin + direction;

        var triangle1 = new Triangle(a, c, b, material);
        var triangle2 = new Triangle(d, b, c, material);

        _triangles = (left: triangle1, right: triangle2);
        Normalized = _triangles.left.Normalized + _triangles.right.Normalized;
    }


    public Material Material { get; }
    public Vector3D Normalized { get; set; }

    public (bool hasHit, double intersectionDistance) NextIntersection(Ray ray)
    {
        var result1 = _triangles.left.NextIntersection(ray);
        if (result1.hasHit)
        {
            Normalized = _triangles.left.Normalized;
            return result1;
        }

        Normalized = _triangles.right.Normalized;
        return _triangles.right.NextIntersection(ray);
    }
}
using Commons.Materials;

namespace Commons._3D;

public interface IObject3D
{
    Material Material { get; }
    Vector3D Normalized { get; set; }
    (bool hasHit, double intersectionDistance) NextIntersection(Ray ray);
}
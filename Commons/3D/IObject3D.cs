namespace Commons._3D;

public interface IObject3D
{
    RgbColor Color { get; }
    Vector3D Normalized { get; set; }
    (bool hasHit, double intersectionDistance) NextIntersection(Ray ray);
}
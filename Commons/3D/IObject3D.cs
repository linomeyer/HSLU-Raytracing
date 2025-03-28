namespace Commons._3D;

public interface IObject3D
{
    RgbColor Color { get; }
    (bool hasHit, double intersectionDistance) NextIntersection(Ray ray);
}
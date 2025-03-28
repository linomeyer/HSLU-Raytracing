namespace Commons._3D;

public interface IObject3D
{
    (bool hasHit, double intersectionDistance) NextIntersection(Ray ray);
}
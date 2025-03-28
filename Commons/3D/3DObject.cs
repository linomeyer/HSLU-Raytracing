namespace Commons._3D;

public interface IObject3D
{
    (bool, double) NextIntersection(Ray ray);
}
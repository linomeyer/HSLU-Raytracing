namespace Commons._3D;

public static class CalcHelper
{
    public static Vector3D IntersectionPoint(Ray ray, double distance) => ray.Origin + ray.Direction * distance;
}
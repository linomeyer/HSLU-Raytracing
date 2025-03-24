using SkiaSharp;

namespace Commons;

public class Circle(Vector2D center, double radius, SKColor color)
{
    public SKColor Color => color;
    public Vector2D Center => center;
    public double Radius => radius;

    public bool Contains(Vector2D point) => point.Distance(center) <= radius;
}
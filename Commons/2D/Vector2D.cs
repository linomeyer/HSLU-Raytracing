namespace Commons._2D;

public class Vector2D(int x, int y)
{
    public int X => x;
    public int Y => y;

    public static Vector2D operator +(Vector2D a, Vector2D b) => new(a.X + b.X, a.Y + b.Y);

    public static Vector2D operator -(Vector2D a, Vector2D b) => new(a.X - b.X, a.Y - b.Y);

    public static Vector2D operator *(Vector2D vector, int scalar) => new(vector.X * scalar, vector.Y * scalar);

    public double Distance(Vector2D vector)
    {
        var distance = this - vector;
        return Math.Sqrt(Math.Pow(distance.X, 2) + Math.Pow(distance.Y, 2));
    }
}
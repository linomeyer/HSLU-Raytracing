﻿namespace Commons;

public class Vector3D(double x, double y, double z)
{
    public double X => x;
    public double Y => y;
    public double Z => z;

    public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

    public static Vector3D operator +(Vector3D a, Vector3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3D operator -(Vector3D a, Vector3D b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3D operator *(Vector3D a, int scalar) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

    public double ScalarProduct(Vector3D other) => X * other.X + Y * other.Y + Z * other.Z;
    public double ScalarProduct(Vector3D other, double angle) => Length * other.Length * Math.Cos(angle);

    public double DotProduct(Vector3D other) => X * other.X + Y * other.Y + Z * other.Z;

    public Vector3D Normalize() => new(X / Length, Y / Length, Z / Length);

    public double EuclideanDistance(Vector3D other)
    {
        var distance = this - other;
        return Math.Sqrt(distance.X * distance.X + distance.Y * distance.Y + distance.Z * distance.Z);
    }
}
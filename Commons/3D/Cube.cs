﻿namespace Commons._3D;

public class Cube : IObject3D
{
    private readonly List<Triangle> _cubeFaces;
    private Vector3D _normalVector;

    public Cube(Vector3D center, double size, RgbColor color, double rotation = 0)
    {
        Center = center;
        Size = size;
        Rotation = rotation;
        Color = color;

        _cubeFaces = CreateCube();
    }

    public Vector3D Center { get; }
    public double Size { get; }
    public double Rotation { get; }
    public RgbColor Color { get; }
    public Vector3D Normalized { get; set; }

    public (bool hasHit, double intersectionDistance) NextIntersection(Ray ray)
    {
        var hasHit = false;
        var minDistance = double.MinValue;
        foreach (var plane in _cubeFaces)
        {
            var (hasHitPlane, intersectionDistance) = plane.NextIntersection(ray);
            if (hasHitPlane && intersectionDistance > minDistance)
            {
                hasHit = true;
                minDistance = intersectionDistance;
                _normalVector = plane.Normalized;
            }
        }

        return (hasHit, minDistance);
    }

    private List<Triangle> CreateCube()
    {
        var edges = CreateEdges();
        for (var i = 0; i < edges.Count; i++) edges[i] = Rotate(edges[i]);

        var cube = new List<Triangle>
        {
            // Front
            new(edges[0], edges[1], edges[2], Color),
            new(edges[0], edges[2], edges[3], Color),
            // Back
            new(edges[4], edges[6], edges[5], Color),
            new(edges[4], edges[7], edges[6], Color),
            // Left
            new(edges[0], edges[3], edges[7], Color),
            new(edges[0], edges[7], edges[4], Color),
            // Right
            new(edges[1], edges[5], edges[6], Color),
            new(edges[1], edges[6], edges[2], Color),
            // Bottom
            new(edges[0], edges[4], edges[5], Color),
            new(edges[0], edges[5], edges[1], Color),
            // Top
            new(edges[3], edges[2], edges[6], Color),
            new(edges[3], edges[6], edges[7], Color)
        };

        return cube;
    }

    private List<Vector3D> CreateEdges()
    {
        var halfSize = Size / 2;
        var edges = new List<Vector3D>();
        edges.Add(new Vector3D(Center.X - halfSize, Center.Y - halfSize, Center.Z - halfSize));
        edges.Add(new Vector3D(Center.X + halfSize, Center.Y - halfSize, Center.Z - halfSize));
        edges.Add(new Vector3D(Center.X + halfSize, Center.Y + halfSize, Center.Z - halfSize));
        edges.Add(new Vector3D(Center.X - halfSize, Center.Y + halfSize, Center.Z - halfSize));

        edges.Add(new Vector3D(Center.X - halfSize, Center.Y - halfSize, Center.Z + halfSize));
        edges.Add(new Vector3D(Center.X + halfSize, Center.Y - halfSize, Center.Z + halfSize));
        edges.Add(new Vector3D(Center.X + halfSize, Center.Y + halfSize, Center.Z + halfSize));
        edges.Add(new Vector3D(Center.X - halfSize, Center.Y + halfSize, Center.Z + halfSize));
        return edges;
    }

    private Vector3D Rotate(Vector3D corner)
    {
        // Convert angle to radians
        var angleRadians = Rotation * Math.PI / 180;

        // Translate point to origin
        var x = corner.X - Center.X;
        var y = corner.Y - Center.Y;
        var z = corner.Z - Center.Z;

        // Rotate around Y-axis
        var cosA = Math.Cos(angleRadians);
        var sinA = Math.Sin(angleRadians);

        var newX = x * cosA - z * sinA;
        var newZ = x * sinA + z * cosA;

        // Rotate around X-axis
        var tiltAngle = 15f * Math.PI / 180; // 15 degrees tilt
        var cosT = Math.Cos(tiltAngle);
        var sinT = Math.Sin(tiltAngle);

        var finalY = y * cosT - newZ * sinT;
        var finalZ = y * sinT + newZ * cosT;

        return new Vector3D(
            newX + Center.X,
            finalY + Center.Y,
            finalZ + Center.Z
        );
    }
}
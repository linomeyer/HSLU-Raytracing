using Commons._3D;

namespace Commons.BVH;

public class BVHWrapper
{
    private readonly List<IObject3D> _nonTriangleObjects;

    private readonly BVHNode _root;

    public BVHWrapper(List<IObject3D> objects)
    {
        // Separate triangles from other objects
        var triangles = new List<Triangle>();
        _nonTriangleObjects = new List<IObject3D>();

        foreach (var obj in objects)
            if (obj is Triangle triangle)
                triangles.Add(triangle);
            else
                _nonTriangleObjects.Add(obj);

        if (triangles.Count > 0) _root = BuildBVH(triangles);
    }

    private BVHNode BuildBVH(List<Triangle> triangles)
    {
        if (triangles.Count == 0) return null;
        if (triangles.Count == 1) return new BVHNode(triangles[0], GetTriangleBounds(triangles[0]));

        var axis = Random.Shared.Next(3);

        triangles.Sort((a, b) =>
        {
            var boxA = GetTriangleBounds(a);
            var boxB = GetTriangleBounds(b);

            var centroidA = axis switch
            {
                0 => boxA.Min.X + (boxA.Max.X - boxA.Min.X) * 0.5,
                1 => boxA.Min.Y + (boxA.Max.Y - boxA.Min.Y) * 0.5,
                _ => boxA.Min.Z + (boxA.Max.Z - boxA.Min.Z) * 0.5
            };

            var centroidB = axis switch
            {
                0 => boxB.Min.X + (boxB.Max.X - boxB.Min.X) * 0.5,
                1 => boxB.Min.Y + (boxB.Max.Y - boxB.Min.Y) * 0.5,
                _ => boxB.Min.Z + (boxB.Max.Z - boxB.Min.Z) * 0.5
            };

            return centroidA.CompareTo(centroidB);
        });

        var mid = triangles.Count / 2;
        var leftTriangles = triangles.GetRange(0, mid);
        var rightTriangles = triangles.GetRange(mid, triangles.Count - mid);

        var left = BuildBVH(leftTriangles);
        var right = BuildBVH(rightTriangles);

        return new BVHNode(left, right);
    }

    private static AABB GetTriangleBounds(Triangle triangle) => new(
        new Vector3D(
            Math.Min(Math.Min(triangle.A.X, triangle.B.X), triangle.C.X),
            Math.Min(Math.Min(triangle.A.Y, triangle.B.Y), triangle.C.Y),
            Math.Min(Math.Min(triangle.A.Z, triangle.B.Z), triangle.C.Z)
        ),
        new Vector3D(
            Math.Max(Math.Max(triangle.A.X, triangle.B.X), triangle.C.X),
            Math.Max(Math.Max(triangle.A.Y, triangle.B.Y), triangle.C.Y),
            Math.Max(Math.Max(triangle.A.Z, triangle.B.Z), triangle.C.Z)
        )
    );

    public (bool hit, double distance, IObject3D hitObj) Intersect(Ray ray)
    {
        var result = (hit: false, distance: double.MaxValue, hitObj: null as IObject3D);

        foreach (var obj in _nonTriangleObjects)
        {
            var nextIntersection = obj.NextIntersection(ray);
            if (nextIntersection.hasHit && nextIntersection.intersectionDistance < result.distance) result = (true, nextIntersection.intersectionDistance, obj);
        }

        if (_root != null) IntersectBVH(_root, ray, ref result);

        return result;
    }

    private void IntersectBVH(BVHNode node, Ray ray, ref (bool hit, double distance, IObject3D hitObj) result)
    {
        if (node == null) return;

        if (!node.Bounds.Hit(ray))
            return;

        if (node.IsLeaf)
        {
            var nextIntersection = node.Triangle.NextIntersection(ray);
            if (nextIntersection.hasHit && nextIntersection.intersectionDistance < result.distance)
                result = (true, nextIntersection.intersectionDistance, node.Triangle);
            return;
        }

        IntersectBVH(node.Left, ray, ref result);
        IntersectBVH(node.Right, ray, ref result);
    }

    private class BVHNode
    {
        public BVHNode(Triangle triangle, AABB bounds)
        {
            Triangle = triangle;
            Bounds = bounds;
        }

        public BVHNode(BVHNode left, BVHNode right)
        {
            Left = left;
            Right = right;
            Bounds = AABB.SurroundingBox(left.Bounds, right.Bounds);
        }

        public AABB Bounds { get; }
        public BVHNode Left { get; }
        public BVHNode Right { get; }
        public Triangle Triangle { get; }
        public bool IsLeaf => Triangle != null;
    }
}
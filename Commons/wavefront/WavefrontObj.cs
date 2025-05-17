using Commons._2D;
using Commons._3D;

namespace Commons.wavefront;

public record WavefrontObj
{
    public List<Triangle> Triangles { get; init; } = [];
    public List<Vector3D> Vertices { get; init; } = [];
    public List<Vector3D> Normals { get; init; } = [];
    public List<Vector2D> TextureCoordinates { get; init; } = [];
}
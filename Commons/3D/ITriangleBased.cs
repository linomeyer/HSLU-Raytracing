namespace Commons._3D;

public interface ITriangleBased : IObject3D
{
    Vector3D Normalized { get; }
    RgbColor Color { get; }
}
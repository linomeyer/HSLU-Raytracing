using Commons._3D;

namespace Commons;

public class Camera(Vector3D focus)
{
    public Ray CreateRay(Vector3D origin) => new(origin, origin - focus);
}
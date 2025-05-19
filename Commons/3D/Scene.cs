using Commons.Lighting;

namespace Commons._3D;

public class Scene(List<IObject3D> objects, List<LightSource> lightSources, int width, int height)
{
    public List<IObject3D> Objects => objects;
    public List<LightSource> LightSources => lightSources;
    public int Width => width;
    public int Height => height;
}
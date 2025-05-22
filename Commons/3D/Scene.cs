using Commons.Lighting;
using Commons.Raytracer;

namespace Commons._3D;

public class Scene(List<IObject3D> objects, List<LightSource> lightSources, Dimensions dimensions)
{
    public List<IObject3D> Objects => objects;
    public List<LightSource> LightSources => lightSources;
    public int Width => dimensions.Width;
    public int Height => dimensions.Height;
}
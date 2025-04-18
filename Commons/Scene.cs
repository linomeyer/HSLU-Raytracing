using Commons._3D;
using Commons.Lighting;

namespace Commons;

public class Scene(List<IObject3D> objects, List<LightSource> lightSources, RgbColor? backgroundColor = null, int reflectionDepth = 10)
{
    public RgbColor BackgroundColor { get; } = backgroundColor ?? RgbColor.Black;
    public List<LightSource> LightSources { get; } = lightSources;
    public List<IObject3D> Objects { get; } = objects;
    public int ReflectionDepth { get; } = reflectionDepth;
}
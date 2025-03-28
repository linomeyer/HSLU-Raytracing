using Commons._3D;
using Commons.Lighting;

namespace Commons;

public class RayTracer(List<IObject3D> sceneObjects, List<LightSource> lightSources)
{
    public List<IObject3D> SceneObjects => sceneObjects;
    public List<LightSource> LightSources => lightSources;

    private static RgbColor AmbientLight => new(0.1, 0.1, 0.1);

    public RgbColor CalcRay(Ray ray)
    {
        var nearestIntersection = double.MaxValue;
        var color = RgbColor.Black;

        foreach (var sceneObject in SceneObjects)
        {
            var (hasHit, intersectionDistance) = sceneObject.NextIntersection(ray);
            if (hasHit && intersectionDistance < nearestIntersection)
            {
                nearestIntersection = intersectionDistance;

                var intersectionPoint = CalcHelper.IntersectionPoint(ray, intersectionDistance);

                color = sceneObject switch
                {
                    Triangle triangle => ColorTriangle(triangle, intersectionPoint),
                    Sphere sphere => ColorSphere(sphere, intersectionPoint),
                    _ => color
                };
            }
        }

        return color;
    }

    private RgbColor ColorSphere(Sphere sphere, Vector3D intersectionPoint)
    {
        var color = RgbColor.Black;
        foreach (var lightSource in LightSources)
        {
            var n = (intersectionPoint - sphere.Center).Normalize();
            var s = (lightSource.Position - intersectionPoint).Normalize();

            var colorFactor = s.ScalarProduct(n) > 0 ? s.ScalarProduct(n) : 0;
            color += new RgbColor(sphere.Color.R, sphere.Color.G, sphere.Color.B) * colorFactor + AmbientLight;
        }

        return color;
    }

    private RgbColor ColorTriangle(Triangle triangle, Vector3D intersectionPoint)
    {
        var color = RgbColor.Black;
        foreach (var lightSource in LightSources)
        {
            var vectorToLightSource = (lightSource.Position - intersectionPoint).Normalize();
            var scalarProductOfNormalizedPLaneToLightSource =
                Math.Max(0, triangle.NormalVector.ScalarProduct(vectorToLightSource));
            color += triangle.Color * lightSource.Color *
                     scalarProductOfNormalizedPLaneToLightSource * lightSource.Intensity
                     + new RgbColor(0.1, 0.1, 0.1);
        }

        return color;
    }
}
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
                    ITriangleBased triangleBased => ColorTriangleBased(triangleBased, intersectionPoint),
                    Sphere sphere => ColorSphere(sphere, intersectionPoint),
                    _ => color
                };
                color += sceneObject.Color * AmbientLight;
            }
        }

        return color;
    }

    private RgbColor ColorSphere(Sphere sphere, Vector3D intersectionPoint)
    {
        var color = RgbColor.Black;
        foreach (var lightSource in LightSources)
        {
            var vectorToLightSource = (lightSource.Position - intersectionPoint).Normalize();
            var intersectionPointIsInShadow = CheckRayIntersectionWithOtherObjects(vectorToLightSource, intersectionPoint, lightSource, sphere);

            if (!intersectionPointIsInShadow)
            {
                var normalized = (intersectionPoint - sphere.Center).Normalize();
                var colorFactor = Math.Max(0, normalized.ScalarProduct(vectorToLightSource));
                color += sphere.Color * lightSource.Color * colorFactor * lightSource.Intensity;
            }
        }

        return color;
    }

    private RgbColor ColorTriangleBased(ITriangleBased triangleBased, Vector3D intersectionPoint)
    {
        var color = RgbColor.Black;
        foreach (var lightSource in LightSources)
        {
            var vectorToLightSource = (lightSource.Position - intersectionPoint).Normalize();
            var intersectionPointIsInShadow = CheckRayIntersectionWithOtherObjects(vectorToLightSource, intersectionPoint, lightSource, triangleBased);

            if (!intersectionPointIsInShadow)
            {
                var colorFactor = Math.Max(0, triangleBased.Normalized.ScalarProduct(vectorToLightSource));
                color += triangleBased.Color * lightSource.Color * colorFactor * lightSource.Intensity;
            }
        }

        return color;
    }

    private bool CheckRayIntersectionWithOtherObjects(Vector3D vectorToLightSource, Vector3D intersectionPoint, LightSource lightSource, IObject3D self)
    {
        var intersectionPointIsInShadow = false;
        var rayToLightSource = new Ray(intersectionPoint + vectorToLightSource * MathConstants.Epsilon, vectorToLightSource);
        var distanceToLightSource = Math.Abs((intersectionPoint - lightSource.Position).Length);
        foreach (var sceneObject in SceneObjects)
        {
            if (sceneObject == self && sceneObject is Sphere) continue; // TODO this is a workaround because sphere intersects itself at the moment

            var (hasHit, intersectionDistance) = sceneObject.NextIntersection(rayToLightSource);

            if (!hasHit) continue;

            var intersectionPointWithOtherObject = CalcHelper.IntersectionPoint(rayToLightSource, intersectionDistance);
            var distanceToIntersectionPointWithOtherObject = Math.Abs((intersectionPoint - intersectionPointWithOtherObject).Length);

            if (distanceToIntersectionPointWithOtherObject < distanceToLightSource)
                intersectionPointIsInShadow = true;
        }

        return intersectionPointIsInShadow;
    }
}
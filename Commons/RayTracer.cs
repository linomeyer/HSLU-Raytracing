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
                color = RgbColor.Black;

                var intersectionPoint = CalcHelper.IntersectionPoint(ray, intersectionDistance);

                foreach (var lightSource in LightSources)
                    color += sceneObject switch
                    {
                        ITriangleBased triangleBased => ColorTriangleBased(triangleBased, intersectionPoint, lightSource),
                        Sphere sphere => ColorSphere(sphere, intersectionPoint, lightSource),
                        _ => color
                    };
                color += sceneObject.Color * AmbientLight;
            }
        }

        return color;
    }

    private RgbColor ColorSphere(Sphere sphere, Vector3D intersectionPoint, LightSource lightSource)
    {
        var vectorToLightSource = (lightSource.Position - intersectionPoint).Normalize();
        var intersectionPointIsInShadow = CheckRayIntersectionWithOtherObjects(vectorToLightSource, intersectionPoint, lightSource);

        if (!intersectionPointIsInShadow)
        {
            var normalized = (intersectionPoint - sphere.Center).Normalize();
            var colorFactor = Math.Max(0, normalized.ScalarProduct(vectorToLightSource));
            return sphere.Color * lightSource.Color * colorFactor * lightSource.Intensity;
        }

        return RgbColor.Black;
    }

    private RgbColor ColorTriangleBased(ITriangleBased triangleBased, Vector3D intersectionPoint, LightSource lightSource)
    {
        var vectorToLightSource = (lightSource.Position - intersectionPoint).Normalize();
        var intersectionPointIsInShadow = CheckRayIntersectionWithOtherObjects(vectorToLightSource, intersectionPoint, lightSource);

        if (!intersectionPointIsInShadow)
        {
            var colorFactor = Math.Max(0, triangleBased.Normalized.ScalarProduct(vectorToLightSource));
            return triangleBased.Color * lightSource.Color * colorFactor * lightSource.Intensity;
        }

        return RgbColor.Black;
    }

    private bool CheckRayIntersectionWithOtherObjects(Vector3D vectorToLightSource, Vector3D intersectionPoint, LightSource lightSource)
    {
        var intersectionPointIsInShadow = false;
        var rayToLightSource = new Ray(intersectionPoint + vectorToLightSource * MathConstants.Epsilon, vectorToLightSource);
        var distanceToLightSource = Math.Abs((intersectionPoint - lightSource.Position).Length);
        foreach (var sceneObject in SceneObjects)
        {
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
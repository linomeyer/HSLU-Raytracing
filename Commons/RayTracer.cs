using Commons._3D;
using Commons.Lighting;

namespace Commons;

public class RayTracer(List<IObject3D> sceneObjects, List<LightSource> lightSources)
{
    public List<IObject3D> SceneObjects => sceneObjects;
    public List<LightSource> LightSources => lightSources;

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

                color = CalcColor(sceneObject, intersectionPoint);
                //color += sceneObject.Color * AmbientLight;
            }
        }

        return color;
    }

    private RgbColor CalcColor(IObject3D sceneObject, Vector3D intersectionPoint)
    {
        var color = new RgbColor(sceneObject.Material.Ambient.R, sceneObject.Material.Ambient.G, sceneObject.Material.Ambient.B);
        foreach (var lightSource in LightSources)
        {
            var vectorToLightSource = (lightSource.Position - intersectionPoint).Normalize();
            var intersectionPointIsInShadow = CheckRayIntersectionWithOtherObjects(vectorToLightSource, intersectionPoint, lightSource, sceneObject);

            if (!intersectionPointIsInShadow)
            {
                var colorFactor = Math.Max(0, sceneObject.Normalized.ScalarProduct(vectorToLightSource));
                color += sceneObject.Material.Diffuse * lightSource.Color * colorFactor * lightSource.Intensity * (1.0 / LightSources.Count);
            }
        }

        return color;
    }

    private bool CheckRayIntersectionWithOtherObjects(Vector3D vectorToLightSource, Vector3D intersectionPoint, LightSource lightSource, IObject3D self)
    {
        var intersectionPointIsInShadow = false;
        var distanceToLightSource = Math.Abs((intersectionPoint - lightSource.Position).Length);
        var rayToLightSource = new Ray(intersectionPoint + vectorToLightSource * MathConstants.Epsilon, vectorToLightSource);
        // var rayToLightSource = new Ray(Offset(intersectionPoint, self.Normalized), vectorToLightSource);
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

    private Vector3D Offset(Vector3D location, Vector3D normal) // TODO doesnt work correctly
    {
        var normalMultiplier = 256.0f;
        var minDistance = 1 / 32; // if value is smaller than this, precision needs to be handled
        var precisionOffset = 1 / ushort.MaxValue; // offset value to handle errors in precision if value is smaller than minDistance

        // multiply normal to get it to a scale that works for the calculations
        var offset = new Vector3D(normal.X * normalMultiplier, normal.Y * normalMultiplier, normal.Z * normalMultiplier);

        // add or subtract offset value depending on location being positive or negative
        var locationWithOffset = new Vector3D(
            location.X + (location.X < 0 ? -offset.X : offset.X),
            location.Y + (location.Y < 0 ? -offset.Y : offset.Y),
            location.Z + (location.Z < 0 ? -offset.Z : offset.Z)
        );

        return new Vector3D(
            Math.Abs(location.X) < minDistance ? location.X + precisionOffset * normal.X : locationWithOffset.X,
            Math.Abs(location.Y) < minDistance ? location.Y + precisionOffset * normal.Y : locationWithOffset.Y,
            Math.Abs(location.Z) < minDistance ? location.Z + precisionOffset * normal.Z : locationWithOffset.Z
        );
    }
}
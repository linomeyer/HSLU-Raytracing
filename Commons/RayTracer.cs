using Commons._3D;
using Commons.Lighting;

namespace Commons;

public class RayTracer(List<IObject3D> sceneObjects, List<LightSource> lightSources, int maxDepth = 6)
{
    public List<IObject3D> SceneObjects => sceneObjects;
    public List<LightSource> LightSources => lightSources;

    public RgbColor CalcRay(Ray ray) => CalcRay(ray, 0);

    private RgbColor CalcRay(Ray ray, int depth)
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

                color = CalcColor(ray, sceneObject, intersectionPoint, depth);
            }
        }

        return color;
    }

    private RgbColor CalcColor(Ray ray, IObject3D sceneObject, Vector3D intersectionPoint, int currentDepth)
    {
        var color = new RgbColor(sceneObject.Material.Ambient.R, sceneObject.Material.Ambient.G, sceneObject.Material.Ambient.B);
        foreach (var lightSource in LightSources)
        {
            var vectorToLightSource = (lightSource.Position - intersectionPoint).Normalize();
            var intersectionPointIsInShadow = CheckRayIntersectionWithOtherObjects(vectorToLightSource, intersectionPoint, lightSource, sceneObject);

            if (!intersectionPointIsInShadow)
            {
                color += AddDiffuseColoring(sceneObject, lightSource, vectorToLightSource);
                color += AddSpecularColoring(sceneObject, lightSource, vectorToLightSource, ray);
            }
        }

        if (sceneObject.Material.Reflectivity > 0 && currentDepth < maxDepth)
        {
            var reflectionDirection = CalcReflectionDir(ray.Direction, sceneObject);
            var reflectionRay = new Ray(intersectionPoint + reflectionDirection * MathConstants.Epsilon, reflectionDirection);

            var reflectionColor = CalcRay(reflectionRay, currentDepth + 1);

            color = color * (1 - sceneObject.Material.Reflectivity) + reflectionColor * sceneObject.Material.Reflectivity;
        }

        return color;
    }

    private RgbColor AddSpecularColoring(IObject3D sceneObject, LightSource lightSource, Vector3D vectorToLightSource, Ray ray)
    {
        if (sceneObject.Material.Shininess > 0)
        {
            var lightIntensity = lightSource.Intensity * (1.0 / LightSources.Count);
            var reflection = CalcReflectionDir(-vectorToLightSource, sceneObject);
            var specularExponent = sceneObject.Material.Shininess * MathConstants.ShininessMultiplier;
            var shininessFactor = Math.Pow(Math.Max(0, reflection.ScalarProduct(-ray.Direction.Normalize())), specularExponent);

            return sceneObject.Material.Specular * lightIntensity * shininessFactor;
        }

        return new RgbColor(0, 0, 0);
    }

    private RgbColor AddDiffuseColoring(IObject3D sceneObject, LightSource lightSource, Vector3D vectorToLightSource)
    {
        var lightIntensity = lightSource.Intensity * (1.0 / LightSources.Count);
        var colorFactor = Math.Max(0, sceneObject.Normalized.ScalarProduct(vectorToLightSource));

        return sceneObject.Material.Diffuse * lightSource.Color * colorFactor * lightIntensity;
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

    private Vector3D CalcReflectionDir(Vector3D direction, IObject3D self) => direction - self.Normalized * (2 * direction.ScalarProduct(self.Normalized));

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
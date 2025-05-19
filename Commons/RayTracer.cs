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
        var color = sceneObject.Material.Ambient;
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

        var reflectionColor = RgbColor.Black;
        if (sceneObject.Material.Reflectivity > 0 && currentDepth < maxDepth)
        {
            var reflectionDirection = CalcReflectionDir(ray.Direction, sceneObject.Normalized);
            var reflectionRay = new Ray(intersectionPoint + reflectionDirection * MathConstants.Epsilon, reflectionDirection);

            reflectionColor = CalcRay(reflectionRay, currentDepth + 1);

            if (sceneObject is not Sphere { IsGlass: true })
                color = color * (1 - sceneObject.Material.Reflectivity) + reflectionColor * sceneObject.Material.Reflectivity;
        }

        if (sceneObject.Material.Transparency > 0 && currentDepth < maxDepth)
        {
            if (sceneObject is Sphere { IsGlass: true } glassSphere)
            {
                var refractionDirection = CalcDirectionOfRefraction(ray.Direction.Normalize(), sceneObject.Normalized, glassSphere.Material.RefractiveIndex);
                var refractionRayG = new Ray(intersectionPoint + refractionDirection * 0.015, refractionDirection);

                var refractionColorF = CalcRay(refractionRayG, currentDepth + 1);

                var r0 = Math.Pow((1 - glassSphere.Material.RefractiveIndex) / (1 + glassSphere.Material.RefractiveIndex), 2);
                var cosTheta = Math.Abs(sceneObject.Normalized.ScalarProduct(-ray.Direction.Normalize()));
                var fresnelFactor = r0 + (1 - r0) * Math.Pow(1 - cosTheta, 5);

                var trueReflection = fresnelFactor;
                var trueTransparency = 1 - fresnelFactor;
                if (trueTransparency > sceneObject.Material.Transparency) trueTransparency = sceneObject.Material.Transparency;

                color = color + (reflectionColor * trueReflection + refractionColorF * trueTransparency);
            }
            else
            {
                var refractionRay = new Ray(intersectionPoint + ray.Direction * 0.01, ray.Direction);
                var refractionColor = CalcRay(refractionRay, currentDepth + 1);
                var transparency = sceneObject.Material.Transparency;
                color = color * (1 - transparency) + refractionColor * transparency;
            }
        }

        return color;
    }

    private Vector3D CalcDirectionOfRefraction(Vector3D rayDirection, Vector3D normal, double refractionValue)
    {
        var cosi = Math.Clamp(rayDirection.ScalarProduct(normal), -1, 1);

        double etai = 1;
        var etat = refractionValue;
        var n = normal;

        if (cosi < 0)
        {
            cosi = -cosi;
        }
        else
        {
            (etai, etat) = (etat, etai);
            n = -normal;
        }

        var eta = etai / etat;
        var k = 1 - Math.Pow(eta, 2) * (1 - Math.Pow(cosi, 2));

        if (k < 0) return CalcReflectionDir(rayDirection, normal); //rayDirection - normal * (2 * rayDirection.ScalarProduct(normal));

        return rayDirection * eta + n * (eta * cosi - Math.Sqrt(k));
    }

    private RgbColor AddSpecularColoring(IObject3D sceneObject, LightSource lightSource, Vector3D vectorToLightSource, Ray ray)
    {
        if (sceneObject.Material.Shininess > 0)
        {
            var lightIntensity = lightSource.Intensity * (1.0 / LightSources.Count);
            var reflection = CalcReflectionDir(vectorToLightSource, sceneObject.Normalized);
            var specularExponent = sceneObject.Material.Shininess * MathConstants.ShininessMultiplier;
            var shininessFactor = Math.Pow(Math.Max(0, reflection.ScalarProduct(ray.Direction.Normalize())), specularExponent);

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

    private Vector3D CalcReflectionDir(Vector3D direction, Vector3D normal) => direction - normal * (2 * direction.ScalarProduct(normal));
}
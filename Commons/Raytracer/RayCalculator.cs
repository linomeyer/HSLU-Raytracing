﻿using Commons._3D;
using Commons.BVH;
using Commons.Lighting;

namespace Commons.Raytracer;

public class RayCalculator
{
    private readonly BVHWrapper? _bvhWrapper;
    private readonly int _maxDepth;
    private readonly Scene _scene;


    public RayCalculator(Scene scene, int maxDepth, bool useBvh = false)
    {
        _scene = scene;
        _maxDepth = maxDepth;
        _bvhWrapper = null;
        if (useBvh)
            _bvhWrapper = new BVHWrapper(scene.Objects);
    }

    public RgbColor Calc(Ray ray) => Calc(ray, 0);

    private RgbColor Calc(Ray ray, int depth)
    {
        var nearestIntersection = double.MaxValue;
        var color = RgbColor.Black;

        if (_bvhWrapper != null)
        {
            var (hit, distance, hitObject) = _bvhWrapper.Intersect(ray);
            if (hit)
            {
                var intersectionPoint = CalcHelper.IntersectionPoint(ray, distance);
                color = CalcColor(ray, hitObject, intersectionPoint, depth);
            }
        }
        else
        {
            foreach (var sceneObject in _scene.Objects)
            {
                var (hasHit, intersectionDistance) = sceneObject.NextIntersection(ray);
                if (hasHit && intersectionDistance < nearestIntersection)
                {
                    nearestIntersection = intersectionDistance;
                    var intersectionPoint = CalcHelper.IntersectionPoint(ray, intersectionDistance);

                    color = CalcColor(ray, sceneObject, intersectionPoint, depth);
                }
            }
        }

        return color;
    }

    private RgbColor CalcColor(Ray ray, IObject3D sceneObject, Vector3D intersectionPoint, int currentDepth)
    {
        var color = sceneObject.Material.Ambient;
        foreach (var lightSource in _scene.LightSources)
        {
            var vectorToLightSource = (lightSource.Position - intersectionPoint).Normalize();
            var intersectionPointIsInShadow = CheckRayIntersectionWithOtherObjects(vectorToLightSource, intersectionPoint, lightSource, sceneObject);

            if (!intersectionPointIsInShadow)
            {
                color += AddDiffuseColoring(sceneObject, lightSource, vectorToLightSource);
                color += AddSpecularColoring(sceneObject, lightSource, vectorToLightSource, ray);
            }
        }

        (var reflectionColor, color) = CalcReflection(sceneObject, ray, intersectionPoint, color, currentDepth);
        color = CalcRefraction(sceneObject, ray, intersectionPoint, reflectionColor, color, currentDepth);

        return color;
    }

    private RgbColor CalcRefraction(IObject3D sceneObject, Ray ray, Vector3D intersectionPoint, RgbColor reflectionColor, RgbColor color, int currentDepth)
    {
        if (sceneObject.Material.Transparency > 0 && currentDepth < _maxDepth)
        {
            if (sceneObject is Sphere { IsGlass: true } glassSphere)
            {
                var refractionDirection = CalcDirectionOfRefraction(ray.Direction.Normalize(), sceneObject.Normalized, glassSphere.Material.RefractiveIndex);
                var refractionRayG = new Ray(intersectionPoint + refractionDirection * 0.015, refractionDirection);

                var refractionColorF = Calc(refractionRayG, currentDepth + 1);

                var r0 = Math.Pow((1 - glassSphere.Material.RefractiveIndex) / (1 + glassSphere.Material.RefractiveIndex), 2);
                var cosTheta = Math.Abs(sceneObject.Normalized.ScalarProduct(-ray.Direction.Normalize()));
                var fresnelFactor = r0 + (1 - r0) * Math.Pow(1 - cosTheta, 5);

                var trueReflection = fresnelFactor;
                var trueTransparency = 1 - fresnelFactor;
                if (trueTransparency > sceneObject.Material.Transparency) trueTransparency = sceneObject.Material.Transparency;

                color += reflectionColor * trueReflection + refractionColorF * trueTransparency;
            }
            else
            {
                var refractionRay = new Ray(intersectionPoint + ray.Direction * 0.01, ray.Direction);
                var refractionColor = Calc(refractionRay, currentDepth + 1);
                var transparency = sceneObject.Material.Transparency;
                color = color * (1 - transparency) + refractionColor * transparency;
            }
        }

        return color;
    }

    private (RgbColor reflectionColor, RgbColor color) CalcReflection(IObject3D sceneObject, Ray ray, Vector3D intersectionPoint,
        RgbColor color, int currentDepth)
    {
        var reflectionColor = RgbColor.Black;
        if (sceneObject.Material.Reflectivity > 0 && currentDepth < _maxDepth)
        {
            var reflectionDirection = CalcReflectionDir(ray.Direction, sceneObject.Normalized);
            var reflectionRay = new Ray(intersectionPoint + reflectionDirection * MathConstants.Epsilon, reflectionDirection);

            reflectionColor = Calc(reflectionRay, currentDepth + 1);

            if (sceneObject is not Sphere { IsGlass: true })
                color = color * (1 - sceneObject.Material.Reflectivity) + reflectionColor * sceneObject.Material.Reflectivity;
        }

        return (reflectionColor, color);
    }


    private Vector3D CalcDirectionOfRefraction(Vector3D rayDirection, Vector3D normal, double x)
    {
        var cosi = Math.Clamp(rayDirection.ScalarProduct(normal), -1, 1);

        double y = 1;
        var n = normal;

        if (cosi < 0)
        {
            cosi *= -1;
        }
        else
        {
            (y, x) = (x, y);
            n *= -1;
        }

        var z = y / x;
        var k = 1 - Math.Pow(z, 2) * (1 - Math.Pow(cosi, 2));

        if (k < 0) return CalcReflectionDir(rayDirection, normal);

        return rayDirection * z + n * (z * cosi - Math.Sqrt(k));
    }

    private RgbColor AddSpecularColoring(IObject3D sceneObject, LightSource lightSource, Vector3D vectorToLightSource, Ray ray)
    {
        if (sceneObject.Material.Shininess > 0)
        {
            var lightIntensity = lightSource.Intensity * (1.0 / _scene.LightSources.Count);
            var reflection = CalcReflectionDir(vectorToLightSource, sceneObject.Normalized);
            var specularExponent = sceneObject.Material.Shininess * MathConstants.ShininessMultiplier;
            var shininessFactor = Math.Pow(Math.Max(0, reflection.ScalarProduct(ray.Direction.Normalize())), specularExponent);

            return sceneObject.Material.Specular * lightIntensity * shininessFactor;
        }

        return new RgbColor(0, 0, 0);
    }

    private RgbColor AddDiffuseColoring(IObject3D sceneObject, LightSource lightSource, Vector3D vectorToLightSource)
    {
        var lightIntensity = lightSource.Intensity * (1.0 / _scene.LightSources.Count);
        var colorFactor = Math.Max(0, sceneObject.Normalized.ScalarProduct(vectorToLightSource));

        return sceneObject.Material.Diffuse * lightSource.Color * colorFactor * lightIntensity;
    }

    private bool CheckRayIntersectionWithOtherObjects(Vector3D vectorToLightSource, Vector3D intersectionPoint, LightSource lightSource, IObject3D self)
    {
        var intersectionPointIsInShadow = false;
        var distanceToLightSource = Math.Abs((intersectionPoint - lightSource.Position).Length);

        var rayToLightSource = new Ray(intersectionPoint + vectorToLightSource * MathConstants.Epsilon, vectorToLightSource);
        foreach (var sceneObject in _scene.Objects)
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
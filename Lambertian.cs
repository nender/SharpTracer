using System;
using static RayTracer.StaticRandom;

namespace RayTracer
{
    class Lambertian : IMaterial
    {
        public Lambertian(Vec3 albedo) {
            Albedo = albedo;
        }
        
        public Lambertian(double r, double g, double b) {
            Albedo = new Vec3(r, g, b);
        }
        
        public (Ray scatter, Vec3 atten)? Scatter(Ray r, HitRecord rec)
        {
            var target = rec.p + rec.normal + RandomInUnitSphere();
            var scattered = new Ray(rec.p, target - rec.p);
            return (scattered, Albedo);
        }
        
        Vec3 RandomInUnitSphere() {
            Vec3 p;
            do {
                p = 2 * new Vec3(DRand(), DRand(), DRand()) - new Vec3(1, 1, 1);
            } while (p.Dot(p) >= 1);
            return p;
        }   
        readonly Vec3 Albedo;
    }
}
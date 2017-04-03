using System;
using static RayTracer.StaticRandom;

namespace RayTracer
{
    class Refractive : IMaterial
    {

        public Refractive(double refractionIndex)
        {
            RefractionIndex = refractionIndex;
        }
        
        public (Ray scatter, Vec3 atten)? Scatter(Ray r, HitRecord rec)
        {
            var reflected = Reflect(r.Direction, rec.normal);
            Vec3 outwardNormal;
            double niOverNt, cosine, reflectProbability;
            if (r.Direction.Dot(rec.normal) > 0) {
                outwardNormal = -rec.normal;
                niOverNt = RefractionIndex;
                cosine = RefractionIndex * r.Direction.Dot(rec.normal) / r.Direction.Length();
            } else {
                outwardNormal = rec.normal;
                niOverNt = 1 / RefractionIndex; 
                cosine = -r.Direction.Dot(rec.normal) / r.Direction.Length();
            }
            
            var maybeRefracted = Refract(r.Direction, outwardNormal, niOverNt);
            if (maybeRefracted != null) {
                   reflectProbability = Schlick(cosine, RefractionIndex);
            } else {
                reflectProbability = 1;
            }
            
            if (DRand() < reflectProbability) {
                return (new Ray(rec.p, reflected), attenuation);
            } else {
                return (new Ray(rec.p, maybeRefracted.Value), attenuation);
            }
        }
        
        readonly static Vec3 attenuation = new Vec3(1,1,1);
        readonly double RefractionIndex;
        
        static double Schlick(double cosine, double refraction)
        {
            var r0 = (1-refraction) / (1+refraction);
            r0 = r0*r0;
            return r0 + (1-r0)*Math.Pow(1-cosine, 5);
        }
        
        static Vec3 Reflect(Vec3 v, Vec3 n)
            => v - 2*v.Dot(n)*n;
            
        static Vec3? Refract(Vec3 v, Vec3 n, double niOverNt)
        {
            var uv = v.Unit();
            var dt = uv.Dot(n);
            double discriminant = 1 - niOverNt*niOverNt*(1-dt*dt);
            if (discriminant > 0)
                return niOverNt*(uv - n*dt) - n*Math.Sqrt(discriminant);
            else
                return null;
        }
    }
}
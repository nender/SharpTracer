using System;

namespace RayTracer
{
    class Reflective : IMaterial
    {
        public Reflective(Vec3 albedo, double fuzziness) {
            Albedo = albedo;
            Fuzziness = fuzziness;
        }
        
        public Reflective(double r, double g, double b, double fuzz) {
            Albedo = new Vec3(r, g, b);
            Fuzziness = fuzz;
        }
        
        public (Ray scatter, Vec3 atten)? Scatter(Ray r, HitRecord rec)
        {
            var reflected = Reflect(r.Direction.Unit(), rec.normal);
            var scattered = new Ray(rec.p, reflected + Fuzziness*randomInUnitSphere());
            if (scattered.Direction.Dot(rec.normal) > 0)
                return  (scattered, Albedo);
            else
                return null;
        }
        
        Vec3 Reflect(Vec3 v, Vec3 n)
            => v - 2*v.Dot(n)*n;
        
        Vec3 randomInUnitSphere() {
            Vec3 p;
            do {
                p = 2 * new Vec3(rand.NextDouble(), rand.NextDouble(), rand.NextDouble()) - new Vec3(1, 1, 1);
            } while (p.Dot(p) >= 1);
            return p;
        }   
        
        readonly Random rand = new Random();
        readonly Vec3 Albedo;
        readonly double Fuzziness;
    }
}
using System;

namespace RayTracer
{
    class Sphere : IHittable
    {
        public Sphere(Vec3 cen, double r, IMaterial material) {
            Center = cen;
            Radius = r;
            Material = material;
        }
        
        readonly Vec3 Center;
        readonly double Radius;
        readonly IMaterial Material;
        
        public HitRecord? Hit(Ray r, double tMin, double tMax) {
            var oc = r.Origin - Center;
            double a = r.Direction.Dot(r.Direction);
            double b = oc.Dot(r.Direction);
            double c = oc.Dot(oc) - Radius*Radius;
            double descriminant = b*b - a*c;
            if (descriminant > 0) {
                double t = (-b - Math.Sqrt(descriminant)) / a;
                if (t < tMax && t > tMin) {
                    var p = r.PointAtParameter(t);
                    var normal = (p - Center) / Radius;
                    return new HitRecord(t, p, normal, Material);
                }
                
                t = (-b + Math.Sqrt(descriminant)) / a;
                if (t < tMax && t > tMin) {
                    var p = r.PointAtParameter(t);
                    var normal = (p -Center) / Radius;
                    return new HitRecord(t, p, normal, Material);
                }
            }
            return null;
        }

        public (Vec3, Vec3) BoundingBox() {
            var p1 = Center + new Vec3(Radius);
            var p2 = Center - new Vec3(Radius);
            return (p1, p2);
        }
    }
}
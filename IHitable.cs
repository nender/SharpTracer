namespace RayTracer
{
    struct HitRecord {
        public HitRecord(double t, Vec3 p, Vec3 normal, IMaterial material, IHitable reflector) {
            this.t = t;
            this.p = p;
            this.normal = normal;
            this.material = material;
            this.reflector = reflector;
        }

        public readonly double t;
        public readonly Vec3 p;
        public readonly Vec3 normal;
        public readonly IMaterial material;
        public readonly IHitable reflector;
    }
    
    interface IHitable
    {
        HitRecord? Hit(Ray r, double tMin, double tMax);
        BoundingBox BoundingBox();
    }
}
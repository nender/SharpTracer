namespace RayTracer
{
    struct HitRecord {
        public HitRecord(double t, Vec3 p, Vec3 normal, IMaterial material) {
            this.t = t;
            this.p = p;
            this.normal = normal;
            this.material = material;
        }

        public readonly double t;
        public readonly Vec3 p;
        public readonly Vec3 normal;
        public readonly IMaterial material;
    }
    
    interface IHitable
    {
        HitRecord? Hit(Ray r, double tMin, double tMax);
        BoundingBox BoundingBox();
    }
}
using System.Collections.Generic;

namespace RayTracer {
    class StaticOctree : IHittable
    {
        private HittableList objects = new HittableList();

        public StaticOctree(IEnumerable<IHittable> objects) {
            this.objects.AddRange(objects);
        }

        public HitRecord? Hit(Ray r, double tMin, double tMax) {
            return objects.Hit(r, tMin, tMax);
        }
    }
}
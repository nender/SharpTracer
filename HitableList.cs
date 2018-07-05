using System.Collections.Generic;

namespace RayTracer
{
    class HitableList : List<IHitable>, IHitable
    {
        public HitRecord? Hit(Ray r, double tMin, double tMax)
        {
            HitRecord? bestRecord = null;
            double closestSoFar = tMax;
            foreach(var hitable in this) {
                var record = hitable.Hit(r, tMin, closestSoFar);
                if (record.HasValue) {
                    closestSoFar = record.Value.t;
                    bestRecord = record.Value;
                }
            }
            return bestRecord;
        }

        public BoundingBox BoundingBox()
            => new BoundingBox(this);
    }
}
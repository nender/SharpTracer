using System;
using System.Linq;
using System.Collections.Generic;

namespace RayTracer {
    /// Axis aligned bounding box
    class BoundingBox {
        public double Width, Height, Depth;
        public Vec3 Midpoint;

        Vec3 Max;
        Vec3 Min;

        public BoundingBox(Vec3 min, Vec3 max) {
            if (max.X <= min.X || max.Y <= min.Y || max.Z <= min.Z)
                throw new ArgumentException("P2 is greater than P1!");
            Max = max;
            Min = min;
        }

        public BoundingBox(Vec3 center, double radius) {
            Max = center + new Vec3(radius);
            Min = center - new Vec3(radius);
        }

        public BoundingBox(IEnumerable<IHitable> objects) {
            const double inf = double.PositiveInfinity;
            var bounds = new double[6] {-inf, -inf, -inf, inf, inf, inf};
            foreach (var o in objects) {
                var box = o.BoundingBox();
                var ibounds = box.Max.Concat(box.Min).ToArray();
                for (int i = 0; i < 3; i++)
                    if (ibounds[i] > bounds[i])
                        bounds[i] = ibounds[i];
                for (int i = 2; i < ibounds.Length; i++)
                    if (ibounds[i] < bounds[i])
                        bounds[i] = ibounds[i];
            }

            Max = new Vec3(bounds[0], bounds[1], bounds[2]);
            Min = new Vec3(bounds[3], bounds[4], bounds[5]);
        }

        public bool Intersect(BoundingBox other) {
            throw new NotImplementedException();
        }

        public bool Intersect(Ray r) {
            Vec3 tvmin = (Min - r.Origin) / r.Direction;
            Vec3 tvmax = (Max - r.Origin) / r.Direction;

            double tmin = Math.Min(tvmin.X, tvmax.X);
            double tmax = Math.Max(tvmin.X, tvmax.X);
            double tymin = Math.Min(tvmin.Y, tvmax.Y);
            double tymax = Math.Max(tvmin.Y, tvmax.Y);

            if ((tmin > tymax) || (tymin > tmax))
                return false;

            tmin = Math.Min(tymin, tmin);
            tmax = Math.Max(tymax, tmax);

            double tzmin = Math.Min(tvmin.Z, tvmax.Z);
            double tzmax = Math.Max(tvmin.Z, tvmax.Z);

            if ((tmin > tzmax) || (tzmin > tmax))
                return false;
            
            return true;
        }
    }
}
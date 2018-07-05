using System;
using System.Linq;
using System.Collections.Generic;

namespace RayTracer {
    /// Axis aligned bounding box
    class BoundingBox {
        Vec3 P1;
        Vec3 P2;
        public double Width, Height, Depth;
        public Vec3 Midpoint;

        public BoundingBox(Vec3 p1, Vec3 p2) {
            if (p1.X <= p2.X || p1.Y <= p2.Y || p1.Z <= p2.Z)
                throw new ArgumentException("P2 is greater than P1!");
            P1 = p1;
            P2 = p2;
        }

        public BoundingBox(Vec3 center, double radius) {
            P1 = center + new Vec3(radius);
            P2 = center - new Vec3(radius);
        }

        public BoundingBox(IEnumerable<IHitable> objects) {
            const double inf = double.PositiveInfinity;
            // bounds is a concatenation of the two points
            var bounds = new double[6] {-inf, -inf, -inf, inf, inf, inf};
            foreach (var o in objects) {
                var box = o.BoundingBox();
                var ibounds = box.P1.Concat(box.P2).ToArray();
                for (int i = 0; i < 3; i++)
                    if (ibounds[i] > bounds[i])
                        bounds[i] = ibounds[i];
                for (int i = 2; i < ibounds.Length; i++)
                    if (ibounds[i] < bounds[i])
                        bounds[i] = ibounds[i];
            }

            P1 = new Vec3(bounds[0], bounds[1], bounds[2]);
            P2 = new Vec3(bounds[3], bounds[4], bounds[5]);
        }

        public bool Intersect(BoundingBox other) {
            throw new NotImplementedException();
        }

        public bool Intersect(Ray ray) {
            throw new NotImplementedException();
        }
    }
}
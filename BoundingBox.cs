using System;
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

        public BoundingBox(IEnumerable<IHittable> items) {
            throw new NotImplementedException();
        }

        public bool Intersect(BoundingBox other) {
            throw new NotImplementedException();
        }

        public bool Intersect(Ray ray) {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Linq;
using System.Collections.Generic;

namespace RayTracer {
    class KDTree : IHittable
    {
        KDNode Root;

        public KDTree(IEnumerable<IHittable> objects) {
            Root = new KDNode(objects);
        }

        public HitRecord? Hit(Ray r, double tMin, double tMax) {
            return Root.Hit(r, tMin, tMax);
        }
    }

    enum SplitAxis {
        X,
        Y,
        Z
    }

    class KDNode {
        BoundingBox BBox;
        List<IHittable> objects = new List<IHittable>();
        KDNode LeftChild;
        KDNode RightChild;

        public KDNode(IEnumerable<IHittable> objects)
        {
            BBox = new BoundingBox(objects);
            var left = new List<IHittable>();
            var right = new List<IHittable>();

            var splitPoint = BBox.Midpoint;
            var axis = chooseSplitAxis(BBox);
            int splitCount = 0;
            foreach (var o in objects) {
                throw new NotImplementedException();
                // if object intersects the plane
                    left.Add(o);
                    right.Add(o);
                // else if left of plane
                    left.Add(o);
                    splitCount += 1;
                // else
                    right.Add(o);
                    splitCount += 1;
            }

            if (splitCount <= objects.Count() / 2)
                return;
            
            LeftChild = new KDNode(left);
            RightChild = new KDNode(right);
        }

        SplitAxis chooseSplitAxis(BoundingBox box) {
            return new Dictionary<SplitAxis, double>() {
                {SplitAxis.X, box.Width},
                {SplitAxis.Y, box.Height},
                {SplitAxis.Z, box.Depth}
            }
            .OrderBy(x => x.Value)
            .Select(x => x.Key)
            .First();
        }

        public HitRecord? Hit(Ray r, double tMin, double tMax)
        {
            throw new NotImplementedException();
        }
    }
}
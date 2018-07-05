using System;
using System.Linq;
using System.Collections.Generic;

namespace RayTracer {
    class KDTree : IHitable {
        KDNode Root;

        public KDTree(IEnumerable<IHitable> objects) {
            Root = new KDNode(objects);
        }

        public HitRecord? Hit(Ray r, double tMin, double tMax)
            => Root.Hit(r, tMin, tMax);

        public BoundingBox BoundingBox()
            => Root?.BBox;
    }

    enum SplitAxis {
        X,
        Y,
        Z
    }

    class KDNode {
        public BoundingBox BBox;
        List<IHitable> objects = new List<IHitable>();
        KDNode LeftChild;
        KDNode RightChild;

        public KDNode(IEnumerable<IHitable> objects) {
            BBox = new BoundingBox(objects);
            var left = new List<IHitable>();
            var right = new List<IHitable>();

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

        public HitRecord? Hit(Ray r, double tMin, double tMax) {
            if (BBox.Intersect(r)) {
                if (LeftChild != null || RightChild != null) {
                    var leftHit = LeftChild?.Hit(r, tMin, tMax);
                    var rightHit = RightChild?.Hit(r, tMin, tMax);
                    if (leftHit?.t <= rightHit?.t)
                        return leftHit;
                    else if (rightHit?.t > leftHit?.t)
                        return rightHit;
                    else
                        return null;
                } else {
                    HitRecord? bestHit = null;
                    foreach (var o in objects) {
                        var hit = o.Hit(r, tMin, tMax);
                        if (hit?.t < bestHit?.t || hit != null && bestHit == null)
                            bestHit = hit;
                    }
                    return bestHit;
                }
            } else {
                return null;
            }
        }

    }
}
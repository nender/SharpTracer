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
        const int ArbitraryPopLimit = 4;
        public BoundingBox BBox;
        List<IHitable> objects = new List<IHitable>();
        KDNode LeftChild;
        KDNode RightChild;

        public KDNode(IEnumerable<IHitable> objects) {
            BBox = new BoundingBox(objects);
            if (objects.Count() <= ArbitraryPopLimit)
                return;

            var left = new List<IHitable>();
            var right = new List<IHitable>();

            Vec3 splitPoint = new Vec3(0);
            foreach (var o in objects) {
                splitPoint += (o.BoundingBox().Midpoint.Value) * 1.0 / objects.Count();
            }
            var splitAxis = chooseSplitAxis(BBox);

            foreach (var o in objects) {
                switch (splitAxis) {
                    case SplitAxis.X:
                        if (splitPoint.X >= o.BoundingBox().Midpoint.Value.X)
                            right.Add(o);
                        else
                            left.Add(o);
                        break;
                    case SplitAxis.Y:
                        if (splitPoint.Y >= o.BoundingBox().Midpoint.Value.Y)
                            right.Add(o);
                        else
                            left.Add(o);
                        break;
                    case SplitAxis.Z:
                        if (splitPoint.Z >= o.BoundingBox().Midpoint.Value.Z)
                            right.Add(o);
                        else
                            left.Add(o);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            LeftChild = new KDNode(left);
            RightChild = new KDNode(right);
        }

        SplitAxis chooseSplitAxis(BoundingBox box) {
            return new Dictionary<SplitAxis, double>() {
                {SplitAxis.X, box.Width.Value},
                {SplitAxis.Y, box.Height.Value},
                {SplitAxis.Z, box.Depth.Value}
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
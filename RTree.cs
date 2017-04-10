using System;
using System.Collections.Generic;
using System.Linq;

namespace RayTracer
{
    class RTree<T>
    {
        public RTree(int maxEntries = 9)
        {
            maxItemsPerNode = Math.Max(4, maxEntries);
            minItemsPerNode = (int) Math.Max(2, Math.Ceiling(maxEntries * 0.4));
            
            root = new RTreeNode<T>(default(T), BoundingBox3.Zero);
        }
        
        public void Insert(T data, BoundingBox3 bounds) {
            (var leaf, var path) = ChooseLeaf(data, bounds);
            
            if (leaf.Children.Count() + 1 < maxItemsPerNode) {
                leaf.AddChild(data, bounds);
                AdjustTree(leaf, path);
                return;
            } else {
                (var left, var right) = SplitNode(leaf);
                AdjustTree(left, path, right);
                if (leaf == root)
                {
                    var newRoot = new RTreeNode<T>(default(T), null);
                    newRoot.AddChild(left);
                    newRoot.AddChild(right);
                }
            }
        }
        
        void AdjustTree(RTreeNode<T> node, List<RTreeNode<T>> path, RTreeNode<T> other = null) {
            var newNode = node;
            var splitNode = other;
            
            int i = path.Count() - 1;
            while (true) {
                if (newNode == root)
                    return;
                
                i -= 1;
                var parent = path[i];
                RTreeNode<T> splitParent = null;
                
                parent.Bounds.Extend(newNode.Bounds);
                
                if (splitNode != null) {
                    if (parent.Children.Count() + 1 <= maxItemsPerNode)
                        parent.AddChild(splitNode.Data, splitNode.Bounds);
                    else
                        (parent, splitParent) = SplitNode(parent);
                }
                
                newNode = parent;
                splitNode = splitParent;
            }
        }
        
        (RTreeNode<T> left, RTreeNode<T> right) SplitNode(RTreeNode<T> toSplit) {
            throw new NotImplementedException();
        }
        
        (RTreeNode<T> node, List<RTreeNode<T>> path) ChooseLeaf(T data, BoundingBox3 bounds)
        {
            var node = root;
            var path = new List<RTreeNode<T>>();
            
            while (true) {
                path.Add(node);
                if (!node.Children.Any())
                    return (node, path);
                
                node = node
                    .Children
                    .Select(x => (node: x, newBounds: x.Bounds.Extend(bounds)))
                    .OrderBy(x => x.node.Bounds.Volume() - x.newBounds.Volume())
                    .ThenBy(x => x.newBounds.Volume())
                    .Select(x => x.node)
                    .First();
            }
        }
        
        RTreeNode<T> root;
        readonly int minItemsPerNode, maxItemsPerNode;
    }

    class BoundingBox3
    {
        public static readonly BoundingBox3 Zero = new BoundingBox3(new Vec3(0,0,0), new Vec3(0,0,0));
        
        public BoundingBox3(Vec3 p1, Vec3 p2) {
            if (p1.Length() < p2.Length()) {
                Min = p1;
                Max = p2;
            } else {
                Min = p2;
                Max = p1;
            }
        }
        
        Vec3 Min { get; }
        Vec3 Max { get; }

        public double Volume() {
            var size = Max - Min;
            return size.X * size.Y * size.Z;
        }
        
        public bool Intersects(BoundingBox3 other)
            => !(Max.X < other.Min.X) &&
               !(Min.X > other.Max.X) &&
               !(Max.Y < other.Min.Y) &&
               !(Min.Y > other.Max.Y) &&
               !(Max.Z < other.Min.Z) &&
               !(Min.Z > other.Max.Z);
            
        public bool Contains(BoundingBox3 other)
            =>  (Max.X > other.Max.X) &&
                (Max.Y > other.Max.Y) &&
                (Max.Z > other.Max.Z) &&
                (Min.X < other.Min.X) &&
                (Min.Y < other.Min.Y) &&
                (Min.Z < other.Min.Z);
        
        public BoundingBox3 Extend(BoundingBox3 other) {
            if (this.Contains(other))
                return this;

            var minx = Math.Min(Min.X, other.Min.X);
            var miny = Math.Min(Min.Y, other.Min.Y);
            var minz = Math.Min(Min.Z, other.Min.Z);
            var min = new Vec3(minx, miny, minz);

            var maxx = Math.Max(Max.X, other.Max.X);
            var maxy = Math.Max(Max.Y, other.Max.Y);
            var maxz = Math.Max(Max.Z, other.Max.Z);
            var max = new Vec3(maxx, maxy, maxz);

            return new BoundingBox3(min, max);
        }
    }
    
    class RTreeNode<T>
    {
        public RTreeNode(T data, BoundingBox3 bounds) {
            Data = data;
            Bounds = bounds;
        }

        public T Data { get; }
        public BoundingBox3 Bounds { get; private set; }

        public IEnumerable<RTreeNode<T>> Children
            => childList ?? Enumerable.Empty<RTreeNode<T>>();
        List<RTreeNode<T>> childList;
        

        public void AddChild(T data, BoundingBox3 bounds) {
            var node = new RTreeNode<T>(data, bounds);
            AddChild(node);
        }
        
        public void AddChild(RTreeNode<T> node) {
            if (childList == null)
                childList = new List<RTreeNode<T>>();

            childList.Add(node);
            Bounds = Bounds.Extend(node.Bounds);
        }
    }
}
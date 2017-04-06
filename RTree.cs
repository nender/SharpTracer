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
            
            root = new RTreeNode<T>();
        }
        
        public void Insert(T data, BoundingBox3 bounds) {
            var leaf = ChooseLeaf(data, bounds);
            if (leaf.Children.Count() + 1 < maxItemsPerNode) {
                // install E?
                ExpandTree(leaf);
                return;
            } else {
                (var left, var right) = SplitNode(leaf);
                ExpandTree(left, right);
                // if node split propogation resulted in the root being split,
                // create a new root whose children are the two nodes resulting
                // from the split
            }
        }
        
        void ExpandTree(RTreeNode<T> node, RTreeNode<T> other = null) {
            var n = node;
            var nn = other;
            
            if (n == root)
                return;
        }
        
        (RTreeNode<T> left, RTreeNode<T> right) SplitNode(RTreeNode<T> toSplit) {
            throw new NotImplementedException();
        }
        
        RTreeNode<T> ChooseLeaf(T data, BoundingBox3 bounds)
        {
            var node = root;
            
            while (true) {
                if (!node.Children.Any())
                    return node;
                
                throw new NotImplementedException();
                // let F be the entry in node whose bounding box F.I needs
                // least enlargement to include E.I
                // Resolve ties by choosing entry with the rectangle of smallest area.
            }
        }
        
        RTreeNode<T> root;
        readonly int minItemsPerNode, maxItemsPerNode;
    }

    class BoundingBox3
    {
        public BoundingBox3(Vec3 p1, Vec3 p2) {
            if (p1.Equals(p2))
                throw new ArgumentOutOfRangeException();
                
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
    }
    
    class RTreeNode<T>
    {
        public T Data { get; }
        public BoundingBox3 Bounds { get; }
        public IList<T> Children { get; }
    }
}
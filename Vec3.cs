using System;

namespace RayTracer
{
    struct Vec3
    {
        public Vec3(double x, double y, double z) {
            X = x;
            Y = y;
            Z = z;
        }
        
        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public double R => X;
        public double G => Y;
        public double B => Z;
        
        public double Length()
            => Math.Sqrt(X*X+Y*Y+Z*Z);
        
        public double SquaredLength()
            => X*X+Y*Y+Z*Z;
        
        public Vec3 Unit()
            => this / Length();
        
        public static Vec3 operator +(Vec3 v) => v;
        public static Vec3 operator -(Vec3 v) => new Vec3(-v.X, -v.Y, -v.Z);
        
        public static Vec3 operator +(Vec3 v1, Vec3 v2)
            => new Vec3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
            
        public static Vec3 operator +(Vec3 v1, double d)
            => new Vec3(v1.X + d, v1.Y + d, v1.Z + d);
            
        public static Vec3 operator -(Vec3 v1, Vec3 v2)
            => new Vec3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
            
        public static Vec3 operator *(Vec3 v1, Vec3 v2)
            => new Vec3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
            
        public static Vec3 operator *(double d, Vec3 v1)
            => new Vec3(v1.X * d, v1.Y * d, v1.Z * d);
        
        public static Vec3 operator *(Vec3 v1, double d)
            => new Vec3(v1.X * d, v1.Y * d, v1.Z * d);
            
        public static Vec3 operator /(Vec3 v1, Vec3 v2)
            => new Vec3(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
            
        public static Vec3 operator /(Vec3 v1, double d)
            => new Vec3(v1.X / d, v1.Y / d, v1.Z / d);
            
        public double Dot(Vec3 other)
            => X * other.X + Y * other.Y + Z * other.Z;
            
        public Vec3 Cross(Vec3 other)
            => new Vec3(
                Y*other.Z - Z*other.Y,
                -(X*other.Z - Z*other.X),
                X*other.Y - Y*other.X
            );
    }
}
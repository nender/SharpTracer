using System;
using System.Collections;
using System.Collections.Generic;

namespace RayTracer
{
    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    struct Vec3 : IEnumerable<double>
    {
        public Vec3(double x, double y, double z) {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3(double v){
            X = Y = Z = v;
        }

        public readonly double X, Y, Z;
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

        public IEnumerator<double> GetEnumerator()
        {
            yield return X;
            yield return Y;
            yield return Z;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return X;
            yield return Y;
            yield return Z;
        }

        new public string ToString()
            => $"<{Math.Round(X, 4)} {Math.Round(Y, 4)} {Math.Round(Z, 4)}>";
    }
}
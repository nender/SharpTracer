using System;
using static RayTracer.StaticRandom;

namespace RayTracer
{
    class Camera
    {
        public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 viewUp, double verticalFOV, double aspect, double aperture, double focusDist) {
            lensRadius = aperture / 2;
            var theta = verticalFOV * Math.PI/180;
            var halfHeight = Math.Tan(theta/2);
            var halfWidth = aspect * halfHeight;
            origin = lookFrom;
            w = (lookFrom - lookAt).Unit();
            u = (viewUp.Cross(w)).Unit();
            v = w.Cross(u);
            lowerLeftCorner = origin - halfWidth*focusDist*u - halfHeight*focusDist*v - focusDist*w;
            horizontal = 2*halfWidth*focusDist*u;
            vertical = 2*halfHeight*focusDist*v;
        }
        
        public Ray getRay(double s, double t) {
            var rd = lensRadius*randomInUnitDisk();
            var offset = u * rd.X + v * rd.Y;
            return new Ray(origin + offset, lowerLeftCorner + s*horizontal + t*vertical - origin - offset);
        }
        
        static Vec3 randomInUnitDisk() {
            Vec3 p;
            do {
                p = 2 * new Vec3(DRand(), DRand(), 0) - new Vec3(1, 1, 0);
            } while (p.Dot(p) >= 1);
            return p;
        }   
        
        readonly Vec3 origin;
        readonly Vec3 lowerLeftCorner;
        readonly Vec3 horizontal;
        readonly Vec3 vertical;
        readonly Vec3 u, v, w;
        readonly double lensRadius;
    }
}
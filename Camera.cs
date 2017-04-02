using System;

namespace RayTracer
{
    class Camera
    {
        public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 viewUp, double verticalFOV, double aspect) {
            Vec3 u, v, w;
            var theta = verticalFOV * Math.PI/180;
            var halfHeight = Math.Tan(theta/2);
            var halfWidth = aspect * halfHeight;
            origin = lookFrom;
            w = (lookFrom - lookAt).Unit();
            u = (viewUp.Cross(w)).Unit();
            v = w.Cross(u);
            lowerLeftCorner = origin - halfWidth * u - halfHeight * v - w;
            horizontal = 2*halfWidth*u;
            vertical = 2*halfHeight*v;
        }
        
        public Ray getRay(double s, double t)
            => new Ray(origin, lowerLeftCorner + s*horizontal + t*vertical - origin);
        
        readonly Vec3 lowerLeftCorner;
        readonly Vec3 horizontal;
        readonly Vec3 vertical;
        readonly Vec3 origin;
    }
}
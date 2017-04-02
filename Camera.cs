namespace RayTracer
{
    class Camera
    {
        public Camera() {
            lowerLeftCorner = new Vec3(-2, -1, -1);
            horizontal = new Vec3(4, 0, 0);
            vertical = new Vec3(0, 2, 0);
            origin = new Vec3(0, 0, 0);
        }
        
        public Ray getRay(double u, double v)
            => new Ray(origin, lowerLeftCorner + u*horizontal + v*vertical - origin);
        
        readonly Vec3 lowerLeftCorner;
        readonly Vec3 horizontal;
        readonly Vec3 vertical;
        readonly Vec3 origin;
    }
}
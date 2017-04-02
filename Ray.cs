namespace RayTracer {
    class Ray
    {
        public Ray(Vec3 a, Vec3 b) {
            A = a;
            B = b;
        }
        
        public Vec3 Origin => A;
        public Vec3 Direction => B;
        public Vec3 PointAtParameter(double t)
            => A + t * B;
        
        Vec3 A, B;
    }
}
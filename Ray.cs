namespace RayTracer {
    class Ray
    {
        public Ray(Vec3 a, Vec3 b) {
            Origin = a;
            Direction = b;
        }
        
        public readonly Vec3 Origin, Direction;

        public Vec3 PointAtParameter(double t)
            => Origin + t * Direction;
    }
}
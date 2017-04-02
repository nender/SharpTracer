namespace RayTracer
{
    interface IMaterial
    {
        (Ray scatter, Vec3 atten)? Scatter(Ray r, HitRecord rec);
    }
}
using System;

namespace RayTracer
{
    class Program
    {
        static Vec3 Color(Ray r, IHitable world, int depth) {
            var hit = world.Hit(r, 0.001, double.MaxValue);
            switch (hit) {
                case HitRecord rec:
                    var scattered = rec.material.Scatter(r, rec);
                    if (depth < 50 && scattered != null) {
                        (Ray scatter, Vec3 atten) = scattered.Value;
                        return scattered.Value.atten * Color(scattered.Value.scatter, world, depth + 1);
                    }
                    else
                        return new Vec3(0,0,0);
                case null:
                    var unit = r.Direction.Unit();
                    var t = 0.5*(unit.Y + 1);
                    return (1 - t) * new Vec3(1,1,1) + t * new Vec3(0.5, 0.7, 1.0);
            }
        }
        
        static Random rand = new Random();
        
        
        static void Main(string[] args)
        {
            int width = 400;
            int height = 200;
            int samples = 100;
            
            Console.WriteLine("P3");
            Console.WriteLine($"{width} {height}");
            Console.WriteLine("255");
            
            var world = new HitableList {
                new Sphere(new Vec3(0,0,-1), 0.5, new Lambertian(0.1, 0.2, 0.5)),
                new Sphere(new Vec3(0,-100.5,-1), 100, new Lambertian(0.8, 0.8, 0)),
                new Sphere(new Vec3(1,0,-1), 0.5, new Reflective(0.8, 0.6, 0.2, fuzz: 0.4)),
                new Sphere(new Vec3(-1,0,-1), 0.5, new Refractive(1.8)),
            };
            
            var lookFrom = new Vec3(3,3,2);
            var lookAt =  new Vec3(0,0,-1);
            var cam = new Camera(
                lookFrom,
                lookAt,
                viewUp: new Vec3(0,1,0),
                verticalFOV: 20,
                aspect: (double) width / height,
                aperture: 0.5,
                focusDist: (lookFrom - lookAt).Length()
            );
            
            for (int j = 0; j < height; j++) {
                for (int i = 0; i < width; i++) {
                    var col = new Vec3(0, 0, 0);
                    for (int s = 0; s < samples; s++) {
                        var u = (i + rand.NextDouble()) / width;
                        var v = ((height - j) + rand.NextDouble()) / height;
                        var r = cam.getRay(u, v);
                        var p = r.PointAtParameter(2);
                        col += Color(r, world, 0);
                    }
                    
                    col /= samples;
                    col = new Vec3(Math.Sqrt(col.X), Math.Sqrt(col.Y), Math.Sqrt(col.Z));
                    var ir = (int) (255.99*col.R);
                    var ig = (int) (255.99*col.G);
                    var ib = (int) (255.99*col.B);
                    Console.WriteLine($"{ir} {ig} {ib}");
                }
            }
        }
    }
}
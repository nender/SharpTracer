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
        
        static IHitable randomScene()
        {
            double rndSqrd()
                => rand.NextDouble() * rand.NextDouble();
            
            Vec3 metalVec()
                => new Vec3(
                    0.5*(1 + rand.NextDouble()),
                    0.5*(1 + rand.NextDouble()),
                    0.5*(1 + rand.NextDouble())
                );
                
            var world = new HitableList()
            {
                new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(0.5, 0.5, 0.5))
            };
            
            for (int a = -11; a < 11; a++) {
                for (int b = -11; b < 11; b++) {
                    var chooseMat = rand.NextDouble();
                    var center = new Vec3(a+0.9*rand.NextDouble(), 0.2, b+0.9*rand.NextDouble());
                    if ((center - new Vec3(4, 0.2, 0)).Length() > 0.9) {
                        if (chooseMat < 0.8) {
                            world.Add(new Sphere(center, 0.2, new Lambertian(rndSqrd(), rndSqrd(), rndSqrd())));
                        } else if (chooseMat < 0.95) {
                            world.Add(new Sphere(center, 0.2, new Reflective(metalVec(), rand.NextDouble())));
                        } else {
                            world.Add(new Sphere(center, 0.2, new Refractive(1.5)));
                        }
                    }
                }
            }

            var x = new[] {
                new Sphere(new Vec3(0, 1, 0), 1.0, new Refractive(1.5)),
                new Sphere(new Vec3(-4, 1, 0), 1.0, new Lambertian(new Vec3(0.4, 0.2, 0.1))),
                new Sphere(new Vec3(4, 1, 0), 1.0, new Reflective(new Vec3(0.7, 0.6, 0.5), 0.0))
            };
            world.AddRange(x);
                    
            return world;
        }
        
        static Random rand = new Random();
        
        static void Main(string[] args)
        {
            int width = 1920;
            int height = 1080;
            int samples = 500;

            Console.WriteLine("P3");
            Console.WriteLine($"{width} {height}");
            Console.WriteLine("255");
            
            var world = randomScene();
            
            var lookFrom = new Vec3(13,2,3);
            var lookAt =  new Vec3(0,0,0);
            var cam = new Camera(
                lookFrom,
                lookAt,
                viewUp: new Vec3(0,1,0),
                verticalFOV: 20,
                aspect: (double) width / height,
                aperture: 0.1,
                focusDist: 10
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
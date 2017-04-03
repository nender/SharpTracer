using System;
using System.Collections.Generic;
using System.Linq;
using static RayTracer.StaticRandom;

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
        
        static IHitable RandomScene()
        {
            double rndSqrd()
                => DRand() * DRand();
            
            Vec3 metalVec()
                => new Vec3(
                    0.5*(1 + DRand()),
                    0.5*(1 + DRand()),
                    0.5*(1 + DRand())
                );
                
            var world = new HitableList()
            {
                new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(0.5, 0.5, 0.5))
            };
            
            for (int a = -11; a < 11; a++) {
                for (int b = -11; b < 11; b++) {
                    var chooseMat = DRand();
                    var center = new Vec3(a+0.9*DRand(), 0.2, b+0.9*DRand());
                    if ((center - new Vec3(4, 0.2, 0)).Length() > 0.9) {
                        if (chooseMat < 0.8) {
                            world.Add(new Sphere(center, 0.2, new Lambertian(rndSqrd(), rndSqrd(), rndSqrd())));
                        } else if (chooseMat < 0.95) {
                            world.Add(new Sphere(center, 0.2, new Reflective(metalVec(), DRand())));
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

        static IEnumerable<(int r, int g, int b)> Render(Camera cam, IHitable world, int width, int height, int samples, int start, int end)
        {
            for (int j = start; j < end; j++) {
                for (int i = 0; i < width; i++) {
                    var col = new Vec3(0, 0, 0);
                    for (int s = 0; s < samples; s++)
                    {
                        var u = (i + DRand()) / width;
                        var v = ((height - j) + DRand()) / height;
                        var r = cam.getRay(u, v);
                        var p = r.PointAtParameter(2);
                        col += Color(r, world, depth: 0);
                    }

                    col /= samples;
                    col = new Vec3(Math.Sqrt(col.X), Math.Sqrt(col.Y), Math.Sqrt(col.Z));
                    var ir = (int)(255.99 * col.R);
                    var ig = (int)(255.99 * col.G);
                    var ib = (int)(255.99 * col.B);
                    yield return (ir, ig, ib);
                }
            }
        }

        static IEnumerable<(int start, int end)> Partition(int height, int parallelDegree)
        {
            if (height % parallelDegree != 0)
                throw new Exception();

            int stepSize = height / parallelDegree;

            for (int i = 0; i < height; i += stepSize)
            {
                yield return (i, i + stepSize - 1);
            }
        }

        static void Main(string[] args)
        {
            int width = 400;
            int height = 200;
            int samples = 400;
            
            var world = RandomScene();
            
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

            var cores = Environment.ProcessorCount;

            var parts = Partition(height, cores).ToArray();
            var colorData =
                parts.AsParallel()
                .AsOrdered()
                .WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                .Select(x => Render(cam, world, width, height, samples, x.start, x.end))
                .SelectMany(x => x);

            Console.WriteLine("P3");
            Console.WriteLine($"{width} {height}");
            Console.WriteLine("255");

            foreach(var rgb in colorData) {
                (int r, int g, int b) = rgb;
                Console.WriteLine($"{r} {g} {b}");
            }
        }
    }
}
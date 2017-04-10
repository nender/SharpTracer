using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using static RayTracer.StaticRandom;

namespace RayTracer
{
    class Program
    {
        static Vec3 Color(Ray r, IHittable world, int depth) {
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
        
        static IHittable RandomScene()
        {
            double rndSqrd()
                => DRand() * DRand();
            
            Vec3 metalVec()
                => new Vec3(
                    0.5*(1 + DRand()),
                    0.5*(1 + DRand()),
                    0.5*(1 + DRand())
                );
                
            var world = new HittableList()
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
            var rtree = new RTree<Sphere>();
            foreach (var s in world) {
                rtree.Insert((Sphere)s, BoundSphere((Sphere)s));
            }
            return world;
        }
        
        static BoundingBox3 BoundSphere(Sphere s)
        {
            var radius = s.Radius;
            var radiusVec = new Vec3(radius, radius, radius);
            var min = s.Center  - radiusVec;
            var max = s.Center + radiusVec;
            return new BoundingBox3(min, max);
        }

        static IEnumerable<(int r, int g, int b)> Render(Camera cam, IHittable world, int width, int height, int samples, int start, int end)
        {
            var watch = new Stopwatch();
            watch.Start();
            var wat = new List<(int, int, int)>();
            for (int j = start; j <= end; j++) {
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
                    wat.Add((ir, ig, ib));
                }
            }
            Console.Error.WriteLine($"Finished rendering chunk of {wat.Count}px in {watch.ElapsedMilliseconds}ms");
            return wat;
        }

        static IEnumerable<(int start, int end)> Partition(int height, int parallelDegree)
        {
            int stepSize = Math.Max(1, height / parallelDegree);
            int i = 0;
            while (true) {
                if (i + stepSize >= height) {
                    yield return (i, height - 1);
                    yield break;
                }
                else
                {
                    yield return (i, i + stepSize - 1);
                    i += stepSize;
                }
            }
        }

        static void WriteToConsole(IEnumerable<string> data, int width, int height)
        {
            Console.WriteLine("P3");
            Console.WriteLine($"{width} {height}");
            Console.WriteLine("255");
            foreach(var line in data)
                Console.WriteLine(line);
        }

        static void WriteToFile(string fileName, IEnumerable<string> data, int width, int height)
        {
            using (var f = new StreamWriter(File.Open(fileName, FileMode.OpenOrCreate)))
            {
                f.WriteLine("P3");
                f.WriteLine($"{width} {height}");
                f.WriteLine("255");
                foreach(var line in data)
                    f.WriteLine(line);
            }
        }

        static void Main(string[] args)
        {
            int width, height, samples;
            string fileName;
            try {
                if (args.Length == 4) {
                    width = int.Parse(args[0]);
                    height = int.Parse(args[1]);
                    samples = int.Parse(args[2]);
                    fileName = args[3];
                } else if (args.Length == 3) {
                    width = int.Parse(args[0]);
                    height = int.Parse(args[1]);
                    samples = int.Parse(args[2]);
                    fileName = null;
                } else {
                    throw new ArgumentOutOfRangeException();
                }
            } catch (Exception) {
                Console.WriteLine("Usage: ray height width samplesPerPx (filename)");
                Console.WriteLine("if output is omitted stdout is used");
                return;
            }
            

            Console.Error.WriteLine($"{DateTime.Now} Rendering default scene at {width}x{height}px with {samples} samples/px");
            var watch = new Stopwatch();
            watch.Start();
            
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
                parts
                .AsParallel()
                .AsOrdered()
                .Select(x => Render(cam, world, width, height, samples, x.start, x.end))
                .SelectMany(x => x)
                .Select(rgb => $"{rgb.r} {rgb.g} {rgb.b}")
                .ToArray();
                
            Console.Error.WriteLine($"{DateTime.Now} Done. Writing pdb data to {fileName}");
            Console.Error.WriteLine($"Total elapsed time: {watch.Elapsed}");
            
            if (fileName != null) {
                WriteToFile(fileName, colorData, width, height);
            } else {
                WriteToConsole(colorData, width, height);
            }
        }
    }
}
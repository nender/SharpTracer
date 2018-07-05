using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using static RayTracer.StaticRandom;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RayTracer
{
    class Program
    {
        static Vec3 CalculateColor(Ray r, IHittable world, int depth) {
            var hit = world.Hit(r, 0.001, double.MaxValue);
            switch (hit) {
                case HitRecord rec:
                    var scattered = rec.material.Scatter(r, rec);
                    if (depth < 50 && scattered != null) {
                        (Ray scatter, Vec3 atten) = scattered.Value;
                        return scattered.Value.atten * CalculateColor(scattered.Value.scatter, world, depth + 1);
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

            var list = new List<IHittable>()
            {
                new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(0.5, 0.5, 0.5))
            };

            for (int a = -11; a < 11; a++) {
                for (int b = -11; b < 11; b++) {
                    var chooseMat = DRand();
                    var center = new Vec3(a+0.9*DRand(), 0.2, b+0.9*DRand());
                    if ((center - new Vec3(4, 0.2, 0)).Length() > 0.9) {
                        if (chooseMat < 0.8) {
                            list.Add(new Sphere(center, 0.2, new Lambertian(rndSqrd(), rndSqrd(), rndSqrd())));
                        } else if (chooseMat < 0.95) {
                            list.Add(new Sphere(center, 0.2, new Reflective(metalVec(), DRand())));
                        } else {
                            list.Add(new Sphere(center, 0.2, new Refractive(1.5)));
                        }
                    }
                }
            }

            var x = new[] {
                new Sphere(new Vec3(0, 1, 0), 1.0, new Refractive(1.5)),
                new Sphere(new Vec3(-4, 1, 0), 1.0, new Lambertian(new Vec3(0.4, 0.2, 0.1))),
                new Sphere(new Vec3(4, 1, 0), 1.0, new Reflective(new Vec3(0.7, 0.6, 0.5), 0.0))
            };
            list.AddRange(x);

            return new KDTree(list);
        }

        static byte[] Render(Camera cam, IHittable world, int width, int height, int samples, int start, int end)
        {
            var watch = new Stopwatch();
            watch.Start();
            var wat = new byte[(end - start + 1) * width * 4];
            var z = 0;
            for (int j = start; j <= end; j++) {
                for (int i = 0; i < width; i++) {
                    var col = new Vec3(0, 0, 0);
                    for (int s = 0; s < samples; s++)
                    {
                        var u = (i + DRand()) / width;
                        var v = ((height - j) + DRand()) / height;
                        var r = cam.getRay(u, v);
                        var p = r.PointAtParameter(2);
                        col += CalculateColor(r, world, depth: 0);
                    }

                    col /= samples;
                    col = new Vec3(Math.Sqrt(col.X), Math.Sqrt(col.Y), Math.Sqrt(col.Z));
                    wat[z] = (byte)(255.99 * col.R);
                    wat[z + 1] = (byte)(255.99 * col.G);
                    wat[z + 2] = (byte)(255.99 * col.B);
                    wat[z + 3] = 255;
                    z += 4;
                }
            }
            Console.Error.WriteLine($"Finished rendering chunk of {wat.Length / 4}px in {watch.ElapsedMilliseconds}ms");
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

        static void WriteToFile(string fileName, byte[] data, int width, int height)
        {
            var img = Image.LoadPixelData<Byte4>(data, width, height);
            using (var f = File.Open(fileName, FileMode.OpenOrCreate))
                img.SaveAsPng(f as Stream);
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
                .ToArray();

            Console.Error.WriteLine($"{DateTime.Now} Done. Writing pdb data to {fileName}");
            Console.Error.WriteLine($"Total elapsed time: {watch.Elapsed}");

            WriteToFile(fileName, colorData, width, height);
        }
    }
}
using System;
using System.Threading;

namespace RayTracer
{
    class StaticRandom
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static double DRand()
            => random.Value.NextDouble();
    }
}


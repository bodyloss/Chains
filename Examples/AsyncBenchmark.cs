
namespace FuncChainsTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Chains;
    using System.Threading;
    using System.Diagnostics;

    /// <summary>
    /// Benchmarks an async chain
    /// </summary>
    public class AsyncBenchmark
    {


        public static void Run()
        {
            Chain<int, double> chain = new Chain<int, double>()
            .Link<int>(x =>
            {
                // Generate random array of ints
                int[] nums = new int[x];
                Random rand = new Random();
                for (int i = 0; i < x; i++)
                {
                    nums[i] = rand.Next(100);
                }
                return nums;
            })
            .Link<int[]>(x =>
            {
                // order the numbers (pointless but why not)
                return x.OrderBy(y => y).ToArray();
            })
            .Link<int[]>(x =>
            {
                // compute the average and return it
                double average = 0.0;
                for (int i = 0; i < x.Length; i++)
                {
                    average += x[i];
                }
                return average / x.Length;
            });


            int count = 0;
            int started = 0;

            chain.ExecuteFinished += (sender, e) =>
            {
                Interlocked.Increment(ref count);
            };

            int iters = 1000000;

            Queue<int> diff = new Queue<int>(iters);

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < iters; i++)
            {
                started++;o
                chain.ExecuteAsync(10);

                diff.Enqueue(started - count);
            }
            sw.Stop();

            Console.WriteLine("Took: " + sw.Elapsed.ToString());
        }

    }
}


namespace FuncChainsTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Chains;
    using System.Diagnostics;

    /// <summary>
    /// Performs a simple benchmark, comparing a normal implementation and a FuncChains one.
    /// The benchmark comprises of parsing a string to an int, creating an array 1..N, then computing the average
    /// </summary>
    public class BenchmarkSimple
    {
        public static void Benchmark()
        {
            Chain<string, double> chain = new Chain<string, double>()
                .Link<string>(x => Int32.Parse(x))
                .Link<int>(x =>
                {
                    int[] output = new int[x];
                    for (int i = 0; i < x; i++)
                    {
                        output[i] = i + 1;
                    }
                    return output;
                })
                .Link<int[]>(x =>
                {
                    double count = 0.0;
                    foreach (int i in x)
                        count += i;
                    return count / (double)x.Length;
                });

            Diag(chain, 5, 10000000, "15");
        }

        private static void Diag(Chain<string, double> chain, int loops, int iters, string input)
        {
            for (int loop = 0; loop < loops; loop++)
            {
                Stopwatch swChain = Stopwatch.StartNew();
                for (int i = 0; i < iters; i++)
                {
                    chain.Execute(input);
                }
                swChain.Stop();

                Stopwatch swLin = Stopwatch.StartNew();
                for (int i = 0; i < iters; i++)
                {
                    LinearImp(input);
                }
                swLin.Stop();

                Console.WriteLine("Chain (" + loop + "): " + swChain.ElapsedMilliseconds);
                Console.WriteLine("Linear (" + loop + "): " + swLin.ElapsedMilliseconds);
                Console.WriteLine();
            }
        }

        private static double LinearImp(string input)
        {
            int amount = Int32.Parse(input);
            int[] output = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                output[i] = i + 1;
            }
            double count = 0.0;
            foreach (int i in output)
                count += i;
            return count;
        }

    }
}

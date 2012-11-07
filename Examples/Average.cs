
namespace FuncChainsTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Chains;

    /// <summary>
    /// Calculates the average of a randomly generated set of numbers
    /// </summary>
    public class Average
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

            // Execute our chain, passing in 100 as the initial argument
            double avg = chain.Execute(100);
            Console.WriteLine("Average int chain finished, result: " + avg);
        }
    }
}

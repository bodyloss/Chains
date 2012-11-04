using System;
using FuncChains;
using System.Diagnostics;
using System.Threading;

namespace FuncChainsTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Chain<string, string> chain = new Chain<string, string>()
				.Link<string>(x => Int32.Parse(x))
				.Link<int>(x => {
						int[] output = new int[x];
						for (int i = 0; i < x; i++) {
							output[i] = i + 1;
						}
						return output;
					})
				.Link<int[]>(x => {
						double count = 0.0;
						foreach (int i in x)
							count += i;
						return count / (double)x.Length;
					})
				.Link<double>(x => {
						Console.WriteLine("Chain sleeping for 2 seconds");
						Thread.Sleep(2000);
						return x;
					})
				.Link<double>(x => x.ToString());

			chain.ExecuteFinished += delegate(object sender, ExecuteFinishedEventArgs<string> e) {
				Console.WriteLine("Event raised. Return value: " + e.Value);
			};

			chain.ExecuteAsync("12");

			Console.Read ();
		}

	    private static void Diag(Chain<string, string> chain, int loops, int iters, string input) {
			for (int loop = 0; loop < loops; loop++) {
				
				Stopwatch swChain = Stopwatch.StartNew();
				for (int i = 0; i < iters; i++) {
					chain.Execute(input);
				}
				swChain.Stop();
				
				Stopwatch swLin = Stopwatch.StartNew();
				for (int i = 0; i < iters; i++) {
					LinearImp(input);
				}
				swLin.Stop();
				
				Console.WriteLine("Chain       (" + loop + "): " + swChain.ElapsedMilliseconds);
				Console.WriteLine("Linear      (" + loop + "): " + swLin.ElapsedMilliseconds);
				Console.WriteLine();
			}
		}

		private static string LinearImp(string input) {
			int amount = Int32.Parse(input);
			int[] output = new int[amount];
			for (int i = 0; i < amount; i++) {
				output[i] = i + 1;
			}
			double count = 0.0;
			foreach (int i in output)
				count += i;
			return count.ToString();
		}
	}
}

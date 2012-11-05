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
            BenchmarkSimple.Benchmark();

			Console.Read ();
		}
	}
}

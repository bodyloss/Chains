using System;
using Chains;
using System.Diagnostics;
using System.Threading;

namespace FuncChainsTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            AsyncBenchmark.Run();

			Console.Read ();
		}
	}
}

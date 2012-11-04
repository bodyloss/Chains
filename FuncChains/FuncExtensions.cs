using System;

namespace FuncChains
{
	public static class FuncExtensions
	{
		public static Func<TIn, TOut> Chain<TPrevIn, TPrevOut, TIn, TOut>(this Func<TPrevIn, TPrevOut> previous) {

		}
	}
}


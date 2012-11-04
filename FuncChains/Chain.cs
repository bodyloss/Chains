using System;
using System.Collections.Generic;

namespace FuncChains
{
	public delegate void ExecuteFinishedHandler<TVal>(object sender, ExecuteFinishedEventArgs<TVal> e);
	public class ExecuteFinishedEventArgs<TVal> : EventArgs {
		public TVal Value;

		public ExecuteFinishedEventArgs(TVal value) {
			this.Value = value;
		}
	}

	public class Chain<TIn, TOut>
	{
		private const double EXPANSION_FACTOR = 1.5;

		private Func<object, object>[] chain = null;
		private int index = 0;

		/// <summary>
		/// Fired when the an asynchronous execution of the chain finishes
		/// </summary>
		public event ExecuteFinishedHandler<TOut> ExecuteFinished = null;

		public Chain ()
		{
			chain = new Func<object, object>[1];
		}

		/// <summary>
		/// Adds the specified function as the next link in the chain
		/// </summary>
		/// <param name='function'>
		/// The function that will be executed as the next link in the chain
		/// </param>
		/// <typeparam name='FInput'>
		/// The type that the specified function will receive as its input
		/// </typeparam>
		public Chain<TIn, TOut> Link<FInput>(Func<FInput, object> function)
		{
			if (chain.Length == index) {
				Func<object, object>[] temp = new Func<object, object>[chain.Length + (int)(chain.Length * EXPANSION_FACTOR)];
				chain.CopyTo(temp, 0);
				chain = temp;
			}
			chain[index] = x => {
				if (!(x is FInput)) {
					throw new ArgumentException("Expected " + typeof(FInput) + " but received " + typeof(x) + " for link " + index);
				}
				return (object)function((FInput)x);
			};
			index++;

			return this;
		}

		public Chain<TIn, TOut> L<FInput>(Func<FInput, object> function) {
			Link<FInput>(function);
			return this;
		}

		public TOut Execute(TIn value) {
			object lastValue = value;
			for (int i = 0; i < index; i++) {
				lastValue = chain[i](lastValue);
			}
			return (TOut) lastValue;
		}

		private void ExecuteAsyncCallback(IAsyncResult ar) {
			Func<TIn, TOut> func = ar.AsyncState as Func<TIn, TOut>;
			TOut result = func.EndInvoke(ar);

			if (ExecuteFinished != null) {
				ExecuteFinished(this, new ExecuteFinishedEventArgs<TOut>(result));
			}
		}
		public void ExecuteAsync(TIn value) {
			Func<TIn, TOut> func = new Func<TIn, TOut>(Execute);
			func.BeginInvoke(value, ExecuteAsyncCallback, func);
		}

		public TOut EndExecute(IAsyncResult asyncResult) {
			return default(TOut);
		}
	}
}


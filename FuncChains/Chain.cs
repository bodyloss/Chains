using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FuncChains
{
	public delegate void ExecuteFinishedHandler<TVal>(object sender, ExecuteFinishedEventArgs<TVal> e);
	public class ExecuteFinishedEventArgs<TVal> : EventArgs {
		public TVal Value;

		public ExecuteFinishedEventArgs(TVal value) {
			this.Value = value;
		}
	}

    /// <summary>
    /// Represents a set of functions that form the links of a chain, each link takes as input the output of the previous link
    /// </summary>
    /// <typeparam name="FIn">The type of the initial input to the chain</typeparam>
    /// <typeparam name="FOut">The type of the eventual output of the last link in the chain</typeparam>
	public class Chain<TIn, TOut>
	{
        // The rate at which the array that holds the functions is expanded
        // A List is not used as that was found to be slightly slower than a direct array
		protected const double EXPANSION_FACTOR = 1.5;

        protected Func<object, object>[] chain = null;
        protected int index = 0;

		/// <summary>
		/// Fired when the an asynchronous execution of the chain finishes
		/// </summary>
		public event ExecuteFinishedHandler<TOut> ExecuteFinished = null;

        /// <summary>
        /// Create a new chain
        /// </summary>
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
        /// <param name="typeCheck">
        /// Whether to add type checking to ensure each link receives the type it is meant to
        /// </param>
		/// <typeparam name='FInput'>
		/// The type that the specified function will receive as its input
		/// </typeparam>
		public virtual Chain<TIn, TOut> Link<FInput>(Func<FInput, object> function, bool typeCheck = false)
		{
            // Check if we need to extend our array that holds functions
			if (chain.Length == index) {
				Func<object, object>[] temp = new Func<object, object>[chain.Length + (int)(chain.Length * EXPANSION_FACTOR)];
				chain.CopyTo(temp, 0);
				chain = temp;
			}
            // Add the function, wrapping it in a second function that casts the argument correctly
            // TODO work out how to avoid using a wrapping function
            if (typeCheck)
            {
                chain[index] = x =>
                {
                    if (!(x is FInput))
                        throw new ArgumentException("Link " + index + " expected " + typeof(FInput) + " but received " + x.GetType());
                    return function((FInput)x);
                };
            }
            else
            {
                chain[index] = x => function((FInput)x);
            }
			index++;

			return this;
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
		public Chain<TIn, TOut> L<FInput>(Func<FInput, object> function) {
			Link<FInput>(function);
			return this;
		}

        /// <summary>
        /// Execute the chain using the given initial argument, returning the eventual outcome
        /// </summary>
        /// <param name="value">Initial argument that is passed to the first link in the chain</param>
        /// <returns>Output of the last link</returns>
		public TOut Execute(TIn value) {
			object lastValue = value;
			for (int i = 0; i < index; i++) {
				lastValue = chain[i](lastValue);
			}
			return (TOut) lastValue;
		}

        /// <summary>
        /// Executes the chain asynchronously using the given argument as the argument to the first link.
        /// The event ExecuteFinished is fired when this completes
        /// </summary>
        /// <param name="value">Initial argument that is passed to the first link in the chain</param>
		public void ExecuteAsync(TIn value) {
			Func<TIn, TOut> func = new Func<TIn, TOut>(Execute);
			func.BeginInvoke(value, ExecuteAsyncCallback, func);
            
		}

        /// <summary>
        /// Callback to execute when the chain finishes
        /// </summary>
        /// <param name="ar"></param>
        private void ExecuteAsyncCallback(IAsyncResult ar)
        {
            Func<TIn, TOut> func = ar.AsyncState as Func<TIn, TOut>;
            TOut result = func.EndInvoke(ar);

            if (ExecuteFinished != null)
            {
                ExecuteFinished(this, new ExecuteFinishedEventArgs<TOut>(result));
            }
        }
	}
}


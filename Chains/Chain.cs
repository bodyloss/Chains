
namespace Chains
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.Remoting.Messaging;
    
    /// <summary>
    /// Represents a set of functions that form the links of a chain, each link takes as input the output of the previous link
    /// </summary>
    /// <typeparam name="FIn">The type of the initial input to the chain</typeparam>
    /// <typeparam name="FOut">The type of the eventual output of the last link in the chain</typeparam>
    [System.Runtime.InteropServices.ComVisible(true)]
	public class Chain<TIn, TOut>
	{
        // The rate at which the array that holds the functions is expanded
        // A List is not used as that was found to be slightly slower than a direct array
		private const double expansionFactor = 1.5;

        // Holds our chain of functions
        private Func<int, object, object>[] chain = null;

        // The location of the next place to insert a function into chain
        private int index = 0;

		/// <summary>
		/// Fired when the an asynchronous execution of the chain finishes
		/// </summary>
		public event EventHandler<ExecuteFinishedEventArgs<TIn, TOut>> ExecuteFinished = null;

        /// <summary>
        /// Create a new chain that accepts TIn and output TOut
        /// </summary>
		public Chain ()
		{
			chain = new Func<int, object, object>[1];
		}

		/// <summary>
		/// Adds the specified func as the next link in the chain
		/// </summary>
		/// <param name='func'>
		/// The func that will be executed as the next link in the chain
		/// </param>
        /// <param name="typeCheck">
        /// Whether to add type checking to ensure each link receives the type it is meant to
        /// </param>
		/// <typeparam name='FInput'>
		/// The type that the specified func will receive as its input
		/// </typeparam>
        public virtual Chain<TIn, TOut> Link<TInput>(Func<TInput, object> func)
        {
            // Check if we need to extend our array that holds functions
            if (chain.Length == index)
            {
                Func<int, object, object>[] temp 
                    = new Func<int, object, object>[chain.Length + (int)(chain.Length * expansionFactor)];
                chain.CopyTo(temp, 0);
                chain = temp;
            }
            // Add the func. Wrapping it in a outer func that type checks, does error handling and calls
            // the inner func with a casted input object.
            // TODO work out how to avoid having to cast
            chain[index] = (link, x) =>
                {
                    if (!(x is TInput))
                        throw new LinkArgumentException(link, typeof(TInput).ToString(), x.GetType().ToString());
                    try
                    {
                        return func((TInput)x);
                    }
                    catch (Exception e)
                    {
                        throw new ChainExecutionException("Exception in link " + link, e);
                    }
                };

            index++;

            return this;
        }

        /// <summary>
        /// Execute the chain using the given initial argument, returning the eventual outcome
        /// </summary>
        /// <param name="value">Initial argument that is passed to the first link in the chain</param>
        /// <returns>Output of the last link</returns>
        /// <exception cref="LinkArgumentException">Thrown if a link receives the wrong type of input, contains the offending link</exception>
        /// <exception cref="ChainExecutionException">Thrown when a link throws an 
        /// exception. Contains the original exception and the offending link</exception>
		public TOut Execute(TIn value) {
            object lastValue = value;
            for (int i = 0; i < index; i++)
            {
                lastValue = chain[i](i, lastValue);
            }
            return (TOut)lastValue;
		}

        /// <summary>
        /// Executes the chain asynchronously using the given argument as the argument to the first link.
        /// The event ExecuteFinished is fired when this completes
        /// </summary>
        /// <param name="value">Initial argument that is passed to the first link in the chain</param>
		public void ExecuteAsync(TIn value) {
            Func<TIn, ChainReturn<TOut>> func = new Func<TIn, ChainReturn<TOut>>(ExecuteWithHandling);
            func.BeginInvoke(value, ExecuteAsyncCallback, value);
		}

        /// <summary>
        /// Callback to execute when the chain finishes
        /// </summary>
        /// <param name="ar"></param>
        private void ExecuteAsyncCallback(IAsyncResult ar)
        {
            AsyncResult asyncResult = ar as AsyncResult;

            // Get our original Func from the async result
            Func<TIn, ChainReturn<TOut>> func = asyncResult.AsyncDelegate as Func<TIn, ChainReturn<TOut>>;

            ChainReturn<TOut> result = func.EndInvoke(ar);

            if (ExecuteFinished != null)
            {
                // Raise the execute finished event
                if (result.Status == ChainStatus.Error)
                {
                    ExecuteFinished(
                        this,
                        new ExecuteFinishedEventArgs<TIn, TOut>(result.Exception)
                        );
                }
                else
                {
                    ExecuteFinished(
                        this,
                        new ExecuteFinishedEventArgs<TIn, TOut>((TIn)ar.AsyncState, result.Output)
                        );
                }
            }
        }

        /// <summary>
        /// Executes a chain, error checking each step and terminating as soon as an error occurs
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private ChainReturn<TOut> ExecuteWithHandling(TIn value)
        {
            object lastValue = value;

            // execute each func in our chain, passing in the current link index and
            // the output from the last func (or the initial value if first link).
            for (int i = 0; i < index; i++)
            {
                try
                {
                    lastValue = chain[i](i, lastValue);
                }
                catch (Exception e)
                {
                    return new ChainReturn<TOut>(e);
                }
            }
            return new ChainReturn<TOut>((TOut)lastValue);
        }
	}
}


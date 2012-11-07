
namespace Chains
{
    using System;
    using System.Collections.Generic;

    public class ExecuteFinishedEventArgs<TIn, TOut> : EventArgs
    {
        public TIn Input { get; set; }
        public TOut Output { get; set; }
        public Exception Exception { get; set; }

        public ExecuteFinishedEventArgs(TIn input, TOut output)
        {
            this.Input = input;
            this.Output = output;
            this.Exception = null;
        }

        public ExecuteFinishedEventArgs(Exception originalException)
        {
            this.Exception = originalException;
        }
    }
}


namespace Chains
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents the execution status of a chain
    /// </summary>
    public enum ChainStatus
    {
        Success,
        Error
    }

    /// <summary>
    /// The output from a chain when it was run asynchronously. Or the Exception if
    /// one occurred during execution.
    /// </summary>
    public class ChainReturn<TValue>
    {
        /// <summary>
        /// Execution Exception if one occurred 
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Output from the chain
        /// </summary>
        public TValue Output { get; set; }

        /// <summary>
        /// Whether the chain executed without error
        /// </summary>
        public ChainStatus Status { get; set; }

        public ChainReturn(TValue value)
        {
            this.Exception = null;
            this.Status = ChainStatus.Success;
            this.Output = value;
        }

        public ChainReturn(Exception exception)
        {
            this.Exception = exception;
            this.Status = ChainStatus.Error;
        }
    }
}

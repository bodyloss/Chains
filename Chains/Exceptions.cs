
namespace Chains
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The exception that is thrown when an error occurs during the execution of one of a chains links.
    /// </summary>
    [Serializable]
    public class ChainExecutionException : Exception
    {
        public ChainExecutionException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// The exception that is thrown when a link receives the wrong input type
    /// </summary>
    [Serializable]
    public class LinkArgumentException : Exception
    {
        public LinkArgumentException(int link, string expectedType, string receivedType)
            : base("Link " + link + " expectedType " + expectedType + " but received " + receivedType) { }
    }
}

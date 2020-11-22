using System;
using System.Runtime.Serialization;

namespace CountSummLib.Exceptions
{
    [Serializable]
    internal class ReportException : Exception
    {
        public ReportException()
        {
        }

        public ReportException(string message) : base(message)
        {
        }

        public ReportException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReportException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
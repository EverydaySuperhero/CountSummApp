using System;
using System.Runtime.Serialization;

namespace CountSummLib.Exceptions
{
    [Serializable]
    public class ReportException : Exception
    {
        public Exception e;
        public ReportException(Exception e)
        {
            this.e = e;
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
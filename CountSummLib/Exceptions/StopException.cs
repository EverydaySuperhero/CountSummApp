using System;
using System.Runtime.Serialization;

namespace CountSummLib.Exceptions
{
    [Serializable]
    internal class StopException : Exception
    {
        public StopException()
        {
        }

        public StopException(string message) : base(message)
        {
        }

        public StopException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StopException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
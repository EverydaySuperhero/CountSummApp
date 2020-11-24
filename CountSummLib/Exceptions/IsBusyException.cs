using System;
using System.Runtime.Serialization;

namespace CountSummLib
{
    [Serializable]
    public class IsBusyException : Exception
    {
        public IsBusyException()
        {
        }

        public IsBusyException(string message) : base(message)
        {
        }

        public IsBusyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IsBusyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
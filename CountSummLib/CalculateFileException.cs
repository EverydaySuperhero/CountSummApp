using System;
using System.Runtime.Serialization;

namespace CountSummLib
{
    [Serializable]
    internal class CalculateFileException : Exception
    {
        public CalculateFileException()
        {
        }

        public CalculateFileException(string message) : base(message)
        {
        }

        public CalculateFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CalculateFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
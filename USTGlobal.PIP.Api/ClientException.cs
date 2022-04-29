using System;
using System.Runtime.Serialization;

namespace USTGlobal.PIP.Api
{
    /// <summary>
    /// CustomException
    /// </summary>
    [Serializable]
    public class ClientException : Exception
    {
        /// <summary>
        /// Throw exception with out message
        /// </summary>
        public ClientException()
        : base() { }

        /// <summary>
        /// Throw exception with simple message
        /// </summary>
        /// <param name="message"></param>
        public ClientException(string message)
            : base(message) { }

        /// <summary>
        /// Throw exception with message format and parameters
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public ClientException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        /// <summary>
        /// Throw exception with simple message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ClientException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Throw exception with message format and inner exception. Note that, the variable length params are always floating.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="innerException"></param>
        /// <param name="args"></param>
        public ClientException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        /// <summary>
        /// Custom exception constructor is used during exception serialization/deserialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ClientException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}

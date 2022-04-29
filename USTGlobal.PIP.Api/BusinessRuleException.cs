using System;
using System.Runtime.Serialization;

namespace USTGlobal.PIP.Api
{
    /// <summary>
    /// BusinessRuleException
    /// </summary>
    [Serializable]
    public class BusinessRuleException : Exception
    {
        /// <summary>
        /// Throw exception with out message
        /// </summary>
        public BusinessRuleException()
        : base() { }

        /// <summary>
        /// Throw exception with simple message
        /// </summary>
        /// <param name="message"></param>
        public BusinessRuleException(string message)
            : base(message) { }

        /// <summary>
        /// Throw exception with message format and parameters
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public BusinessRuleException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        /// <summary>
        /// Throw exception with simple message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public BusinessRuleException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Throw exception with message format and inner exception. Note that, the variable length params are always floating.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="innerException"></param>
        /// <param name="args"></param>
        public BusinessRuleException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        /// <summary>
        /// Custom exception constructor is used during exception serialization/deserialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected BusinessRuleException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}

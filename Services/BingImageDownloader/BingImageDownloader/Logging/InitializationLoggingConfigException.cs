using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BingImageDownloader.Logging
{
    [Serializable]
    public class InitializationLoggingConfigException : ApplicationException
    {
        public InitializationLoggingConfigException()
        {
        }

        public InitializationLoggingConfigException(string message)
            : base(message)
        {
        }

        public InitializationLoggingConfigException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InitializationLoggingConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

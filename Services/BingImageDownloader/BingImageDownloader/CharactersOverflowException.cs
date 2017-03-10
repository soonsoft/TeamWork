using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BingImageDownloader
{
    /// <summary>
    /// 字符长度超长异常
    /// </summary>
    [Serializable]
    public class CharactersOverflowException : ApplicationException
    {
        public CharactersOverflowException()
        {
        }

        public CharactersOverflowException(string message)
            : base(message)
        {
        }

        public CharactersOverflowException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CharactersOverflowException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace BingImageDownloader.Logging
{
    public static class LogExtension
    {
        public static void Debug(this Log log, object message)
        {
            log.Current.Debug(message);
        }

        public static void Debug(this Log log, object message, Exception exception)
        {
            log.Current.Debug(message, exception);
        }

        public static void Info(this Log log, object message)
        {
            log.Current.Info(message);
        }

        public static void Info(this Log log, object message, Exception exception)
        {
            log.Current.Info(message, exception);
        }

        public static void Warn(this Log log, object message)
        {
            log.Current.Warn(message);
        }

        public static void Warn(this Log log, object message, Exception exception)
        {
            log.Current.Warn(message, exception);
        }

        public static void Error(this Log log, object message)
        {
            log.Current.Error(message);
        }

        public static void Error(this Log log, object message, Exception exception)
        {
            log.Current.Error(message, exception);
        }

        public static void Fatal(this Log log, object message)
        {
            log.Current.Fatal(message);
        }

        public static void Fatal(this Log log, object message, Exception exception)
        {
            log.Current.Fatal(message, exception);
        }
    }
}

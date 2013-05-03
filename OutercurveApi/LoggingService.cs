using System;
using System.Runtime.CompilerServices;
using ClrPlus.Core.Extensions;
using ServiceStack.Logging;

namespace Outercurve.Api
{
    public class LoggingService
    {
        private readonly ILog _log;
        private object _lock = new object();
        public LoggingService(ILog log)
        {
            _log = log;
        }

        public void StartLog(object request, [CallerMemberName] string memberName = "",
                             [CallerFilePath] string sourceFilePath = "",
                             [CallerLineNumber] int sourceLineNumber = 0)
        {
            lock (_lock)
            {
                _log.Info("---------------------------------------------------------------------------------");
                var type = request.GetType();

                _log.Info(CreateLocationInfo(memberName, sourceFilePath, sourceLineNumber));
                _log.InfoFormat("class {0}:", type.FullName);
                foreach (var prop in type.GetProperties())
                {
                    _log.InfoFormat("   {0}={1}", prop.Name, prop.GetValue(request));
                }
            }


        }

        private string CreateLocationInfo(string memberName, string sourceFilePath, int sourceLineNumber)
        {
            return "{0},{1}:{2}".format(memberName, sourceFilePath, sourceLineNumber);
        }

        public void Debug(object message, [CallerMemberName] string memberName = "",
                          [CallerFilePath] string sourceFilePath = "",
                          [CallerLineNumber] int sourceLineNumber = 0)
        {
            lock (_lock)
            {
                _log.DebugFormat("{0}-{1}",CreateLocationInfo(memberName, sourceFilePath, sourceLineNumber), message);
            }
        }

        public void Debug(object message, Exception exception, [CallerMemberName] string memberName = "",
                          [CallerFilePath] string sourceFilePath = "",
                          [CallerLineNumber] int sourceLineNumber = 0)
        {
            lock (_lock)
            {
                _log.DebugFormat("{0}-{1}-{2}-{3}", CreateLocationInfo(memberName, sourceFilePath, sourceLineNumber),
                                 message, exception.Message, exception.StackTrace);
            }
        }

        


        public void Error(object message, [CallerMemberName] string memberName = "",
                          [CallerFilePath] string sourceFilePath = "",
                          [CallerLineNumber] int sourceLineNumber = 0)
        {

        }

        public void Error(object message, Exception exception, [CallerMemberName] string memberName = "",
                          [CallerFilePath] string sourceFilePath = "",
                          [CallerLineNumber] int sourceLineNumber = 0)
        {
        }


        public void Fatal(object message, [CallerMemberName] string memberName = "",
                          [CallerFilePath] string sourceFilePath = "",
                          [CallerLineNumber] int sourceLineNumber = 0)
        {
        }

        public void Fatal(object message, Exception exception, [CallerMemberName] string memberName = "",
                          [CallerFilePath] string sourceFilePath = "",
                          [CallerLineNumber] int sourceLineNumber = 0)
        {
        }


        public void Info(object message, [CallerMemberName] string memberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
        }

        public void Info(object message, Exception exception, [CallerMemberName] string memberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
        }


        public void Warn(object message, [CallerMemberName] string memberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
        }

        public void Warn(object message, Exception exception, [CallerMemberName] string memberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
        }
    }
}
using System;
using System.Runtime.CompilerServices;
using ServiceStack.Logging;
using SigningServiceBase;

namespace Outercurve.SigningApi
{
    public class LoggingService : ILoggingService
    {
        private readonly ILog _log;
        private readonly object _lock = new object();
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


        public void StartAuthenticate(string userName, string password, bool succeeded, [CallerMemberName] string memberName = "",
                                      [CallerFilePath] string sourceFilePath = "",
                                      [CallerLineNumber] int sourceLineNumber = 0)
        {
            lock (_lock)
            {
                _log.InfoFormat("AUTHENTICATE,{0},{1},{2},{3},{4},{5}", userName, password, succeeded, memberName, sourceFilePath, sourceLineNumber);
            }
        }

        private static string CreateLocationInfo(string memberName, string sourceFilePath, int sourceLineNumber)
        {
            return String.Format("{0},{1}:{2}",memberName, sourceFilePath, sourceLineNumber);
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
            lock (_lock)
            {
                _log.ErrorFormat("{0}-{1}", CreateLocationInfo(memberName, sourceFilePath, sourceLineNumber), message);
            }
        }

        public void Error(object message, Exception exception, [CallerMemberName] string memberName = "",
                          [CallerFilePath] string sourceFilePath = "",
                          [CallerLineNumber] int sourceLineNumber = 0)
        {
            lock (_lock)
            {
                _log.ErrorFormat("{0}-{1}-{2}-{3}", CreateLocationInfo(memberName, sourceFilePath, sourceLineNumber),
                                 message, exception.Message, exception.StackTrace);
            }
        }


        public void Fatal(object message, [CallerMemberName] string memberName = "",
                          [CallerFilePath] string sourceFilePath = "",
                          [CallerLineNumber] int sourceLineNumber = 0)
        {
            lock (_lock)
            {
                _log.FatalFormat("{0}-{1}", CreateLocationInfo(memberName, sourceFilePath, sourceLineNumber), message);
            }
        }

        public void Fatal(object message, Exception exception, [CallerMemberName] string memberName = "",
                          [CallerFilePath] string sourceFilePath = "",
                          [CallerLineNumber] int sourceLineNumber = 0)
        {
            lock (_lock)
            {
                _log.FatalFormat("{0}-{1}-{2}-{3}", CreateLocationInfo(memberName, sourceFilePath, sourceLineNumber),
                                 message, exception.Message, exception.StackTrace);
            }
        }


        public void Info(object message, [CallerMemberName] string memberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
            lock (_lock)
            {
                _log.InfoFormat("{0}-{1}", CreateLocationInfo(memberName, sourceFilePath, sourceLineNumber), message);
            }
        }

        public void Info(object message, Exception exception, [CallerMemberName] string memberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
            lock (_lock)
            {
                _log.InfoFormat("{0}-{1}-{2}-{3}", CreateLocationInfo(memberName, sourceFilePath, sourceLineNumber),
                                 message, exception.Message, exception.StackTrace);
            }
        }


        public void Warn(object message, [CallerMemberName] string memberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
            lock (_lock)
            {
                _log.WarnFormat("{0}-{1}", CreateLocationInfo(memberName, sourceFilePath, sourceLineNumber), message);
            }
        }

        public void Warn(object message, Exception exception, [CallerMemberName] string memberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
            lock (_lock)
            {
                _log.WarnFormat("{0}-{1}-{2}-{3}", CreateLocationInfo(memberName, sourceFilePath, sourceLineNumber),
                                 message, exception.Message, exception.StackTrace);
            }
        }
    }
}
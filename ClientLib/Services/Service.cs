using System;
using System.Collections.Generic;
using System.Linq;
using Outercurve.ClientLib.Messages;
using ServiceStack.ServiceClient.Web;

namespace Outercurve.ClientLib.Services
{
    public abstract class Service
    {
        protected JsonServiceClient Client;
        protected Action<Message> SendMessage;
        protected Action<ProgressMessage> SendProgress;

        protected Service(string username, string password, string serviceUrl, Action<Message> messageHandler = null,
                       Action<ProgressMessage> progressHandler =
                           null)
        {
            
            
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }

            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

           
            Initialize(serviceUrl, messageHandler, progressHandler);

            Client.UserName = username;
            Client.Password = password;
            
        }

        protected Service(string serviceUrl, Action<Message> messageHandler = null,
                          Action<ProgressMessage> progressHandler =
                              null)
        {
            Initialize(serviceUrl, messageHandler, progressHandler);
        }

        private void Initialize(string serviceUrl, Action<Message> messageHandler,
                                Action<ProgressMessage> progressHandler)
        {
            SendMessage = messageHandler ?? (m => { });
            SendProgress = progressHandler ?? (m => { });
            Client = new JsonServiceClient(serviceUrl) {Timeout = TimeSpan.FromMinutes(15)};
        }

        protected void ThrowErrors(IEnumerable<string> errors)
        {

            var errArray = (errors ?? Enumerable.Empty<string>()).ToArray();

            if (!errArray.Any())
            {
                return;
            }
            if (errArray.Length == 1)
            {
                throw new Exception(errArray[0]);
            }
            else
            {
                throw new AggregateException(errArray.Select(i => new Exception(i)));
            }
                
        }

    }
}

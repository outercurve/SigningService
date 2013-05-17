using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ServiceStack.ServiceHost;

namespace Outercurve.DTO.Request
{
    public abstract class BaseRequest<TRequest> : IReturn<TRequest>
    {
        protected BaseRequest()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version;
        }

        public Version Version { get; private set; }
    }
}

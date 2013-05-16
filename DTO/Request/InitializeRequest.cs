using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outercurve.DTOs.Response;
using ServiceStack.ServiceHost;

namespace Outercurve.DTOs.Request
{
    [Route("/initialize")]
    public class InitializeRequest : IReturn<BaseResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

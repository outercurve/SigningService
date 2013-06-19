using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outercurve.DTO.Response;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Outercurve.DTO.Request
{
    [Route("/get-status")]
    [Authenticate]
    [RequiredRole("signers", ApplyTo = ApplyTo.All)]
    public class GetStatus : BaseRequest<GetStatusResponse>
    {
        public string Container { get; set; }
        public string Path { get; set; }
    }
}

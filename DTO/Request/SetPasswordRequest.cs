using Outercurve.DTOs.Response;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Outercurve.DTOs.Request
{
    [Route("/set-password")]
    [Authenticate]
    public class SetPasswordRequest : IReturn<BaseResponse>
    {
        public string NewPassword { get; set; }
    }
}

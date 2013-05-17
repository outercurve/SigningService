using Outercurve.DTO.Response;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Outercurve.DTO.Request
{
    [Route("/set-password")]
    [Authenticate]
    public class SetPasswordRequest : BaseRequest<BaseResponse>
    {
        public string NewPassword { get; set; }
    }
}

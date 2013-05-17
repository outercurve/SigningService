using Outercurve.DTO.Response;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Outercurve.DTO.Request
{
    [Route("/remove-user")]
    [Authenticate]
    [RequiredRole("admins")]
    public class RemoveUserRequest : BaseRequest<BaseResponse>
    {
        public string UserName { get; set; }
    }
}

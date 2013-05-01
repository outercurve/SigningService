using Outercurve.DTOs.Response;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Outercurve.DTOs.Request
{
    [Route("/remove-user")]
    [Authenticate]
    [RequiredRole("admins")]
    public class RemoveUserRequest : IReturn<BaseResponse>
    {
        public string UserName { get; set; }
    }
}

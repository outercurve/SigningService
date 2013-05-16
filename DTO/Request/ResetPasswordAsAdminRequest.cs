using Outercurve.DTOs.Response;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Outercurve.DTOs.Request
{
    [Route("/set-passwordasadmin")]
    [Authenticate]
    [RequiredRole("admins", ApplyTo = ApplyTo.All)]
    public class ResetPasswordAsAdminRequest : IReturn<CreateUserResponse>
    {
        public string UserName { get; set; }
    }
}

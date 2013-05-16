using Outercurve.DTO.Response;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Outercurve.DTO.Request
{
    [Route("/create-user")]
    [Authenticate]
    [RequiredRole("admins")]
    public class CreateUserRequest : IReturn<CreateUserResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

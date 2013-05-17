using System.Collections.Generic;
using Outercurve.DTO.Response;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Outercurve.DTO.Request
{
    [Route("/set-roles")]
    [Authenticate]
    [RequiredRole("admins")]
    public class SetRolesRequest : ModifyRolesRequest
    {
    }

    [Route("/unset-roles")]
    [Authenticate]
    [RequiredRole("admins")]
    public class UnsetRolesRequest : ModifyRolesRequest
    {
    }

    [Route("/get-roles")]
    [Authenticate]
    [RequiredRole("admins")]
    public class GetRolesRequest : BaseRequest<GetRolesResponse>
    {
        public string UserName { get; set; }
    }



    public abstract class ModifyRolesRequest : BaseRequest<BaseResponse>
    {
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
    }
}

using System.Collections.Generic;
using Outercurve.DTOs.Response;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Outercurve.DTOs.Request
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
    public class GetRolesRequest : IReturn<GetRolesResponse>
    {
        public string UserName { get; set; }
    }



    public abstract class ModifyRolesRequest : IReturn<BaseResponse>
    {
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
    }
}

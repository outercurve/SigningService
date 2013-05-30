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

    [Route("/get-rolesasadmin")]
    [Authenticate]
    [RequiredRole("admins")]
    public class GetRolesAsAdminRequest : BaseRequest<GetRolesResponse>
    {
        public string UserName { get; set; }
    }


    [Route("/get-roles")]
    [Authenticate]
    public class GetRolesRequest : BaseRequest<GetRolesResponse>
    {
    }



    public abstract class ModifyRolesRequest : BaseRequest<BaseResponse>
    {
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
    }
}

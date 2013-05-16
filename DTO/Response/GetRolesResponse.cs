using System.Collections.Generic;

namespace Outercurve.DTO.Response
{
    public class GetRolesResponse :BaseResponse
    {
        public List<string> Roles { get; set; }
    }
}

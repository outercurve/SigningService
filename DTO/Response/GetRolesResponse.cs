using System.Collections.Generic;

namespace Outercurve.DTOs.Response
{
    public class GetRolesResponse :BaseResponse
    {
        public List<string> Roles { get; set; }
    }
}

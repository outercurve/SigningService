using Outercurve.DTO.Response;
using ServiceStack.ServiceHost;

namespace Outercurve.DTO.Request
{
    [Route("/initialize")]
    public class InitializeRequest : IReturn<BaseResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

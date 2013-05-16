using Outercurve.DTO.Response;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Outercurve.DTO.Request
{
    [Route("/get-uploadlocation")]
    [Authenticate]
    [RequiredRole("signers", ApplyTo = ApplyTo.All)]
    public class GetUploadLocationRequest : IReturn<GetUploadLocationResponse>
    {
    }
}
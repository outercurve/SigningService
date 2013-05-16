using Outercurve.DTOs.Response;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Outercurve.DTOs.Request
{
    [Route("/get-uploadlocation")]
    [Authenticate]
    [RequiredRole("signers", ApplyTo = ApplyTo.All)]
    public class GetUploadLocationRequest : IReturn<GetUploadLocationResponse>
    {
    }
}
using Outercurve.DTO.Response;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Outercurve.DTO.Request
{
    [Route("/set-codesignature")]
    [Authenticate]
    [RequiredRole("signers", ApplyTo = ApplyTo.All)]
    public class SetCodeSignatureRequest : IReturn<SetCodeSignatureResponse>
    {
        
        public string Container { get; set; }
        public string Path { get; set; }
        public bool StrongName { get; set; }
    }

  
}

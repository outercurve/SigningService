using Outercurve.DTOs.Services.Azure;
using ServiceStack.Configuration;

namespace Outercurve.Api
{
    public class AzureClient
    {
        
        private readonly AzureService _root;
        
        public AzureClient(AppSettings settings)
        {
            var account = settings.GetString("AzureAccount");
            var key = settings.GetString("AzureKey");
            _root = new AzureService(account, key);

        }
        public IAzureService GetRoot()
        {
            return _root;
        }
    }
}
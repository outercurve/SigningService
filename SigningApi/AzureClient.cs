using Outercurve.DTO.Services.Azure;
using ServiceStack.Configuration;

namespace Outercurve.SigningApi
{
    public class AzureClient
    {
        
        private readonly IAzureService _root;
        
        public AzureClient(AppSettings settings)
        {
            var account = settings.GetString("AzureAccount");
            if (account == "--SAMPLE--")
            {
                _root = AzureService.UseStorageEmulator();
            }
            else
            {
                var key = settings.GetString("AzureKey");
                _root = new AzureService(account, key);    
            }
            

        }
        public IAzureService GetRoot()
        {
            return _root;
        }
    }

}
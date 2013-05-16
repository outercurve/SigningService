using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Outercurve.DTO.Services.Azure
{
    public interface IAzureSharedAccessPolicy
    {
        DateTimeOffset? SharedAccessExpiry { get; }
    }

    public class AzureSharedAccessPolicy : IAzureSharedAccessPolicy
    {
        private readonly SharedAccessBlobPolicy _policy;
        internal AzureSharedAccessPolicy(SharedAccessBlobPolicy policy)
        {
            _policy = policy;
        }

        public DateTimeOffset? SharedAccessExpiry { get { return _policy.SharedAccessExpiryTime; }}
    }
}

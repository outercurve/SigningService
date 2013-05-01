using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Outercurve.DTOs.Services.Azure
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

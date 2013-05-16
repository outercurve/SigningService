using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Outercurve.DTO.Services.Azure
{
    public interface IAzureContainer
    {
        string Name { get; }
        Uri Uri { get; }
        IEnumerable<IAzureBlob> Files { get; }
        void SetPermissions(BlobContainerPermissions permissions);
        string GetSharedAccessSignature(string policyName);
        IAzureBlob GetBlob(string name);
        IDictionary<string, IAzureSharedAccessPolicy> GetSharedAccessPolicies();
        void Delete();

        /// <summary>
        /// So I don't lose my mind
        /// </summary>
        IDictionary<string, IAzureSharedAccessPolicy> Policies { get; }
    }

    public class AzureContainer : IAzureContainer
    {
        private readonly CloudBlobContainer _container;
        internal AzureContainer(CloudBlobContainer container)
        {
            _container = container;
        }

        public string Name { get { return _container.Name; }}
        public Uri Uri { get { return _container.Uri; } }

        public void SetPermissions(BlobContainerPermissions permissions)
        {
            _container.SetPermissions(permissions);
        }

        public string GetSharedAccessSignature(string policyName)
        {
            return _container.GetSharedAccessSignature(new SharedAccessBlobPolicy(), policyName);
        }

        public IAzureBlob GetBlob(string name)
        {
            return new AzureBlob(_container.GetBlockBlobReference(name));
        }

        public IEnumerable<IAzureBlob> Files { get { return _container.ListBlobs(useFlatBlobListing: true).OfType<CloudBlockBlob>().Select(i => new AzureBlob(i));} }

        public IDictionary<string, IAzureSharedAccessPolicy> GetSharedAccessPolicies()
        {
            return _container.GetPermissions()
                             .SharedAccessPolicies.Select(
                                 kv =>
                                 new KeyValuePair<string, IAzureSharedAccessPolicy>(kv.Key,
                                                                                    new AzureSharedAccessPolicy(kv.Value)))
                             .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        

        public void Delete()
        {
            _container.Delete();
        }

        public IDictionary<string, IAzureSharedAccessPolicy> Policies { get { return GetSharedAccessPolicies(); } }
    }
}
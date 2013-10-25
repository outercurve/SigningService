using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Outercurve.DTO.Services.Azure
{
    public interface IAzureService
    {
        IAzureContainer CreateContainerIfDoesNotExist(string containerName);
        string Account { get; }
        IEnumerable<IAzureContainer> Containers { get; }
        string SasToken { get; }
        IAzureContainer GetContainer(string name);
    }

    

    public class AzureService : IAzureService
    {
        private readonly CloudBlobClient _client;
        private string _account;
        private AzureService(CloudBlobClient client)
        {
            _client = client;
            
        }

        public static IAzureService UseStorageEmulator()
        {
            return new AzureService();
        }

        public AzureService(string account, string azureKey)
        {
            _account = account;
            var storageAccount = new CloudStorageAccount(new StorageCredentials(account, azureKey), true);
            _client = storageAccount.CreateCloudBlobClient();
        }

        public AzureService(string account, string azureKey, string blobUri)
        {
            _account = account;
            var storageAccount = new CloudStorageAccount(new StorageCredentials(account, azureKey), new Uri(blobUri),
                                                         null, null);
            _client = storageAccount.CreateCloudBlobClient();
        }

        internal AzureService()
        {
            _client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();
            _account = CloudStorageAccount.DevelopmentStorageAccount.Credentials.AccountName;
        }

        public static IAzureService CreateDeveloperSasService(string account, string sasToken)
        {

            account = CloudStorageAccount.DevelopmentStorageAccount.Credentials.AccountName;
            var storageAccount = new CloudStorageAccount(new StorageCredentials(sasToken),
                                                         new Uri(String.Format("http://127.0.0.1:10000/{0}/",account)), null,
                                                                 null);
            return new AzureService(storageAccount.CreateCloudBlobClient()) {_account = account};
        }

        public static IAzureService CreateSasService(string account, string sasToken)
        {
            
            var storageAccount = new CloudStorageAccount(new StorageCredentials(sasToken),
                                                         new Uri(String.Format("https://{0}.blob.core.windows.net",account)), null, null);
            return new AzureService(storageAccount.CreateCloudBlobClient()) {_account = account};
        }

        public IAzureContainer CreateContainerIfDoesNotExist(string containerName)
        {
            var containerRef = _client.GetContainerReference(containerName);
            var c = new AzureContainer(containerRef);
            containerRef.CreateIfNotExists();
            return c;
        }

        public string Account { get { return _account; } }


        public string SasToken { get { return _client.Credentials.SASToken; } }

        public IAzureContainer GetContainer(string name)
        {
            return new AzureContainer(_client.GetContainerReference(name));
        }
        public IEnumerable<IAzureContainer> Containers { get { return _client.ListContainers().Select(cont => new AzureContainer(cont)); } }
    }

    
}
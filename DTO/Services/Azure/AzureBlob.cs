using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Outercurve.DTO.Services.Azure
{
    public interface IAzureBlob
    {
        Stream OpenRead();
        Stream OpenWrite();
        void UploadTo(Stream s);
        string Uri { get; }
        string Name { get; }
    }

    public class AzureBlob : IAzureBlob
    {
        private readonly CloudBlockBlob _blob;
        public AzureBlob(CloudBlockBlob blob)
        {
            _blob = blob;
            
        }

        public Stream OpenRead()
        {
            return _blob.OpenRead();
        }

        public Stream OpenWrite()
        {
            return _blob.OpenWrite();
        }

        public void UploadTo(Stream s)
        {
            _blob.UploadFromStream(s);
        }

        public string Uri { get { return _blob.Uri.ToString(); } }

        public string Name { get { return _blob.Name; } }
    }
}
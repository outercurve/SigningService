using System.IO;
using Outercurve.DTO.Services.Azure;

namespace Outercurve.ClientLib.IoItem
{
    class AzureItem : IIoItem
    {
        private readonly AzureBlob _blob;
        public AzureItem(AzureBlob blob)
        {
            _blob = blob;
        }

        public string FullName { get { return _blob.Uri; } }
        public Stream OpenRead()
        {
            return _blob.OpenRead();
        }

        public Stream OpenWrite()
        {
           return _blob.OpenWrite();
        }



        public string Name
        {
            get { return _blob.Name; }
        }
    }
}

using System.IO;
using ClrPlus.Powershell.Provider.Utility;
using Outercurve.DTOs.Services.Azure;

namespace Outercurve.ToolsLib.IoItem
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

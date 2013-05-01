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
        public ProgressStream OpenRead()
        {
            return new ProgressStream(_blob.OpenRead());
        }

        public ProgressStream OpenWrite()
        {
           return new ProgressStream(_blob.OpenWrite());
        }



        public string Name
        {
            get { return _blob.Name; }
        }
    }
}

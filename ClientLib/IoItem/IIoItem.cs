using System.IO;
using System.IO.Abstractions;
using Outercurve.DTO.Services.Azure;

namespace Outercurve.ClientLib.IoItem
{
    interface IIoItem
    {
        string FullName { get; }
        string Name { get; }
        Stream OpenRead();
        Stream OpenWrite();
    }

    static class IoItemFactory
    {
        public static IIoItem Create(object input)
        {
            
            if (input is AzureBlob)
            {
                return new AzureItem(input as AzureBlob);
            }
            if (input is FileInfoBase)
            {
                return new FileItem(input as FileInfoBase);
            }

            return null;
        }
    }
}

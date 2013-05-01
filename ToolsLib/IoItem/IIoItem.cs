using System.IO;
using ClrPlus.Powershell.Provider.Utility;
using Outercurve.DTOs.Services.Azure;

namespace Outercurve.ToolsLib.IoItem
{
    interface IIoItem
    {
        string FullName { get; }
        string Name { get; }
        ProgressStream OpenRead();
        ProgressStream OpenWrite();
    }

    static class IoItemFactory
    {
        public static IIoItem Create(object input)
        {
            
            if (input is AzureBlob)
            {
                return new AzureItem(input as AzureBlob);
            }
            if (input is FileInfo)
            {
                return new FileItem(input as FileInfo);
            }

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outercurve.DTO.Services.Azure;

namespace SigningServiceBase
{
    public interface IAzureClient : IDependency
    {
        IAzureService GetRoot();
        string CopyFileToTemp(string container, string file);
        void CopyFileToAzure(string container, string file, string tempFile);
        IAzureBlob GetBlob(string container, string fileName);
    }
}

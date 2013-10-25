using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outercurve.DTO.Services.Azure;

namespace SigningServiceBase
{
    public abstract class AzureClientBase : IAzureClient
    {
        private readonly IFileSystem _fs;


        protected IAzureService Root;
        

        protected AzureClientBase(IFileSystem fs)
        {
            
        }

        public IAzureService GetRoot()
        {
            return Root;
        }

        public string CopyFileToTemp(string container, string file)
        {
            var blob = GetBlob(container, file);
            if (blob == null)
                return null;

            var temp = _fs.CreateTempPath(file.Replace('/', '_'));

            using (var blobStream = blob.OpenRead())
            {
                using (var tempFile = _fs.File.OpenWrite(temp))
                {
                    blobStream.CopyTo(tempFile);
                }
            }

            return temp;
        }

        public void CopyFileToAzure(string container, string file, string tempFile)
        {
            var blob = GetBlob(container, file);

            using (var fileStream = _fs.File.OpenRead(tempFile))
            {
                blob.UploadTo(fileStream);
            }
        }

        public IAzureBlob GetBlob(string container, string fileName)
        {
            var cont = Root.Containers.FirstOrDefault(c => c.Name == container);
            if (cont == null)
                return null;

            var file = cont.GetBlob(fileName);
            return file;
        }
    }
}

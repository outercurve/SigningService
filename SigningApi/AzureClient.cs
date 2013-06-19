using System.Linq;
using Outercurve.DTO.Services.Azure;
using ServiceStack.Configuration;

namespace Outercurve.SigningApi
{
    public class AzureClient
    {
        private readonly FsService _fs;

        private readonly IAzureService _root;
        
        public AzureClient(AppSettings settings, FsService fs)
        {
            _fs = fs;
            var account = settings.GetString("AzureAccount");
            if (account == "--SAMPLE--")
            {
                _root = AzureService.UseStorageEmulator();
            }
            else
            {
                var key = settings.GetString("AzureKey");
                _root = new AzureService(account, key);    
            }
            

        }
        
        public IAzureService GetRoot()
        {
            return _root;
        }

        public string CopyFileToTemp(string container, string file)
        {
            var blob = GetBlob(container, file);
            if (blob == null)
                return null;

            var temp = _fs.CreateTempPath(file.Replace('/', '_'));

            using (var blobStream = blob.OpenRead())
            {
                using (var tempFile = _fs.OpenWrite(temp))
                {
                    blobStream.CopyTo(tempFile);
                }
            }

            return temp;
        }

        public void CopyFileToAzure(string container, string file, string tempFile)
        {
            var blob = GetBlob(container, file);

            using (var fileStream = _fs.OpenRead(tempFile))
            {
                blob.UploadTo(fileStream);
            }
        }

        public IAzureBlob GetBlob(string container, string fileName)
        {
            var cont = _root.Containers.FirstOrDefault(c => c.Name == container);
            if (cont == null)
                return null;

            var file = cont.GetBlob(fileName);
            return file;
        }
    }

}
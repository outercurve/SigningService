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
        string GetText();
        void SaveTo(string s);
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

        public string GetText()
        {
            using (var s = OpenRead())
            {
                var r = new StreamReader(s);
                return r.ReadToEnd();
            }
        }

        /// <summary>
        /// from http://stackoverflow.com/questions/1879395/how-to-generate-a-stream-from-a-string
        /// </summary>
        /// <param name="s"></param>
        public void SaveTo(string s)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(s);
                writer.Flush();
                stream.Position = 0;

                UploadTo(stream);
            }
        }

        public string Uri { get { return _blob.Uri.ToString(); } }

        public string Name { get { return _blob.Name; } }
    }
}
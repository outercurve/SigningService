using System;
using System.IO;
using System.Web;
using ServiceStack.Configuration;
using ServiceStack.Logging;

namespace Outercurve.SigningApi
{
    public class FsService
    {
        private readonly AppSettings _settings;
        public FsService(AppSettings settings)
        {
            _settings = settings;
        }

        public Stream OpenRead(string filePath)
        {
            return File.OpenRead(filePath);
        }
        public Stream OpenWrite(string filePath)
        {
            return File.OpenWrite(filePath);
        }


        public string CreateTempPath(string fileName)
        {
            var tempFolder = _settings.GetString("TempFolder") != null ? HttpContext.Current.Server.MapPath(_settings.GetString("TempFolder")) : Path.GetTempPath();
            LogManager.GetLogger(GetType()).DebugFormat("temp folder is: {0}", tempFolder);
            return Path.Combine(tempFolder, Guid.NewGuid().ToString() + fileName);
        }
    }
}
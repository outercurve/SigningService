using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using ClrPlus.Powershell.Provider.Utility;
using SigningServiceBase;

namespace Outercurve.SigningApi.Certificate.FileCertificate
{
    public class FileCertificateService : ICertificateService
    {
        private readonly HttpServerUtilityBase _serverBase;
        private readonly IFileSystem _fileSystem;
        private readonly string _path;
        private readonly string _password;


        public FileCertificateService(HttpServerUtilityBase serverBase, IFileSystem fileSystem, string path, string password)
        {
            _serverBase = serverBase;
            _fileSystem = fileSystem;
            _path = path;
            _password = password;
        }

        public X509Certificate2 Get()
        {
            var realPath = _path;
            if (_path.StartsWith("~"))
            {
                realPath = _serverBase.MapPath(_path);
            }
            var rawData = _fileSystem.File.ReadAllBytes(realPath);
            return new X509Certificate2(rawData, _password, X509KeyStorageFlags.Exportable);
        }
    }
}
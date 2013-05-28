using System.Security.Cryptography.X509Certificates;
using ClrPlus.Windows.PeBinary.Utility;
using ServiceStack.Logging;

namespace Outercurve.SigningApi.Signers
{
    

    public class AuthenticodeSigner
    {
        private LoggingService _loggingService;
        private
        const BinaryLoadOptions BINARY_LOAD_OPTIONS = BinaryLoadOptions.PEInfo |
            BinaryLoadOptions.VersionInfo |
            BinaryLoadOptions.Managed |
            BinaryLoadOptions.Resources |
            BinaryLoadOptions.Manifest |
            BinaryLoadOptions.UnsignedManagedDependencies |
            BinaryLoadOptions.MD5;

        public AuthenticodeSigner(X509Certificate2 certificate, LoggingService log) {
            Certificate = certificate;
            _loggingService = log;
        }
        public X509Certificate2 Certificate {get; private set;}

        public void Sign(string path, bool strongName = false)
        {
          
          /*  try
            {
                

                try
                {
                    _loggingService.Debug("LoggingInAuthenticodeSign");
                    //LogManager.GetLogger(GetType()).DebugFormat("path is {0}", path);
                    var certRef = new CertificateReference(Certificate);
                    _loggingService.Debug("Going to load binary at {0}".format(path));
                    /*
                    if (new FileInfo(path).Length < 4)
                    {
                        throw new InvalidFileToSignException();
                    }*/
                    if (strongName)
                    {
                        using (var wrap = new StrongNameCertificateWrapper(Certificate))
                        {
                            wrap.Sign(path);
                        }

                    }

                    AuthenticodeCertificateWrapper.SignUsingDefaultTimeStampUrls(path, Certificate);
/*
                    r = BinaryLoad(path);
                    _loggingService.Debug("Binary at {0} loaded".format(path));
                    //LogManager.GetLogger(GetType()).DebugFormat("filename of Binary is {0}", r.Filename);
                    r.SigningCertificate = certRef;
                    if (strongName)
                        r.StrongNameKeyCertificate = certRef;

                    _loggingService.Debug("Going to do the signing");
                    
                    _loggingService.Debug("signgin finished successfully");
                }

                catch (AggregateException ae)
                {
                    _loggingService.Debug("Something is wrong!");
                    foreach (var i in ae.Flatten().InnerExceptions)
                    {
                        _loggingService.Error("Bad", i);
                    }

                    if (ae.Flatten()
                          .InnerExceptions.OfType<DigitalSignFailure>()
                          .Any(dsf => dsf.Win32Code == 2148204547))
                    {
                        throw new InvalidFileToSignException();
                    }
                    throw;
                }
            }*/
        }

        private Binary BinaryLoad(string path)
        {
            LogManager.GetLogger(GetType()).DebugFormat("path is {0}", path);
            return Binary.Load(path, BINARY_LOAD_OPTIONS).Result;
        }
    }
}

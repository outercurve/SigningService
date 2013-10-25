using System.Security.Cryptography.X509Certificates;

using ServiceStack.Logging;
using SigningServiceBase;

namespace Outercurve.SigningApi.Signers
{
    

    public class AuthenticodeSigner : IDependency
    {
        private readonly ILoggingService _loggingService;
      

        public AuthenticodeSigner(X509Certificate2 certificate, ILoggingService log) {
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

                    _loggingService.Debug("Signing {0} with {1}".format(path, Certificate.SerialNumber));
                    AuthenticodeCertificateWrapper.SignUsingDefaultTimeStampUrls(path, Certificate, _loggingService);
                    _loggingService.Debug("Finished signing {0} with {1}".format(path, Certificate.SerialNumber));
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Autofac;
using SigningServiceBase;

namespace Outercurve.SigningApi
{
    public class AuthenticodeCertificateSigner : IDependency
    {
        private readonly IFs _fs;
        private readonly IContainer _container;

        public AuthenticodeCertificateSigner(IFs fs, IContainer container)
        {
            _fs = fs;
            _container = container;
        }


        public void SignUsingDefaultTimeStampUrls(string filename, X509Certificate2 cert)
        {
            _fs.TryHardToMakeFileWriteable(filename);

            var urls = new[] {
                "http://timestamp.verisign.com/scripts/timstamp.dll", "http://timestamp.comodoca.com/authenticode", "http://www.startssl.com/timestamp", "http://timestamp.globalsign.com/scripts/timstamp.dll", "http://time.certum.pl/"
            };

            var signedOk = false;
            // try up to three times each url if we get a timestamp error
            for (var i = 0; i < urls.Length * 3; i++)
            {
                var url = urls[i % urls.Length];
                try
                {
                    using (var wrap = _container.Resolve<AuthenticodeCertificateWrapper>( new NamedParameter("certificate", cert)))
                    {

                        if (loggingService != null)
                            loggingService.Debug("Going to sign and timestamp with {0} for {1}".format(url, filename));
                        wrap.Sign(filename, urls[i % urls.Length]);
                        if (loggingService != null)
                            loggingService.Debug("Sign and timestamp worked with {0} for {1}".format(url, filename));
                        // whee it worked!
                        signedOk = true;
                        break;
                    }


                }
                catch (FailedTimestampException)
                {
                    if (loggingService != null)
                        loggingService.Debug("Failed sign and timestamp with {0} for {1}".format(url, filename));
                    continue;
                }
            }

            if (!signedOk)
            {
                // we went thru each one 3 times, and it never signed?
                throw new FailedTimestampException(filename, "All of them!");
            }
        }
    }
}
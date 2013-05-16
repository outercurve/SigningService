using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ClrPlus.Core.Extensions;

namespace Outercurve.Api.Signers
{
    public class OPCSigner 
    {
        public OPCSigner(X509Certificate2 certificate, LoggingService log) {
            Certificate = certificate;
            _log = log;
        }

        private LoggingService _log;

        public X509Certificate2 Certificate {get; private set;}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="overrideCurrentSignature"></param>
        /// <from>http://msdn.microsoft.com/en-us/library/system.io.packaging.packagedigitalsignaturemanager.sign(v=vs.100).aspx</from>
        public void Sign(string path, bool overrideCurrentSignature)
        {
            {
                _log.Debug("We're going to try signing {0}, override current signature {1}".format(path,
                                                                                                   overrideCurrentSignature));
                var package = Package.Open(path);

                _log.Debug("Opened {0}".format(path));
                var signatureManager = new PackageDigitalSignatureManager(package)
                    {
                        CertificateOption = CertificateEmbeddingOption.InSignaturePart
                    };

                if (signatureManager.IsSigned)
                {
                    if (overrideCurrentSignature)
                    {
                        _log.Debug("{0} is signed we'll try to remove signatures".format(path));
                        //TODO: make smarter so we only remove signatures for the relevant parts
                        signatureManager.RemoveAllSignatures();
                        package.Flush();
                    }
                    else
                    {
                        _log.Debug("{0} is signed, we're going to throw".format(path));
                        throw new AlreadySignedException();
                    }
                }

                var toSign = package.GetParts().Select(packagePart => packagePart.Uri).ToList();

                toSign.Add(PackUriHelper.GetRelationshipPartUri(signatureManager.SignatureOrigin));
                toSign.Add(signatureManager.SignatureOrigin);
                toSign.Add(PackUriHelper.GetRelationshipPartUri(new Uri("/", UriKind.RelativeOrAbsolute)));

                _log.Debug("About to start signing {0}".format(path));
                signatureManager.Sign(toSign, Certificate);
                _log.Debug("signed {0}, going to close".format(path));
                package.Close();

                _log.Debug("closed {0}".format(path));
            }
            GC.Collect();
        }
    }
}

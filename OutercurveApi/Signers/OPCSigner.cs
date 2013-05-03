using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Outercurve.Api.Signers
{
    public class OPCSigner 
    {
        public OPCSigner(X509Certificate2 certificate) {
            Certificate = certificate;
        }

        public X509Certificate2 Certificate {get; private set;}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="overrideCurrentSignature"></param>
        /// <from>http://msdn.microsoft.com/en-us/library/system.io.packaging.packagedigitalsignaturemanager.sign(v=vs.100).aspx</from>
        public void Sign(string path, bool overrideCurrentSignature) {
            
            var package = Package.Open(path);


            var signatureManager = new PackageDigitalSignatureManager(package)
                {
                    CertificateOption = CertificateEmbeddingOption.InSignaturePart
                };

            if (signatureManager.IsSigned)
            {
                if (overrideCurrentSignature)
                {
                    //TODO: make smarter so we only remove signatures for the relevant parts
                    signatureManager.RemoveAllSignatures();
                    package.Flush();
                }
                else
                {
                    throw new AlreadySignedException();
                }
            }

            var toSign = package.GetParts().Select(packagePart => packagePart.Uri).ToList();

            toSign.Add(PackUriHelper.GetRelationshipPartUri(signatureManager.SignatureOrigin));
            toSign.Add(signatureManager.SignatureOrigin);
            toSign.Add(PackUriHelper.GetRelationshipPartUri(new Uri("/", UriKind.RelativeOrAbsolute)));
            
            signatureManager.Sign(toSign, Certificate);
            package.Close();
           
        }
    }
}

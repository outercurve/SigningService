using System;
using System.Security.Cryptography.X509Certificates;
using ClrPlus.Windows.PeBinary.Utility;
using ServiceStack.Logging;

namespace Outercurve.Api.Signers
{
    

    public class AuthenticodeSigner
    {
        private
        const BinaryLoadOptions BINARY_LOAD_OPTIONS = BinaryLoadOptions.PEInfo |
            BinaryLoadOptions.VersionInfo |
            BinaryLoadOptions.Managed |
            BinaryLoadOptions.Resources |
            BinaryLoadOptions.Manifest |
            BinaryLoadOptions.UnsignedManagedDependencies |
            BinaryLoadOptions.MD5;

        public AuthenticodeSigner(X509Certificate2 certificate) {
            Certificate = certificate; 
        }
        public X509Certificate2 Certificate {get; private set;}

        public void Sign(string path, bool strongName = false)
        {
            try
            {


                LogManager.GetLogger(GetType()).DebugFormat("path is {0}", path);
                var certRef = new CertificateReference(Certificate);
                var r = BinaryLoad(path);
                LogManager.GetLogger(GetType()).DebugFormat("filename of Binary is {0}", r.Filename);
                r.SigningCertificate = certRef;
                if (strongName)
                    r.StrongNameKeyCertificate = certRef;

                r.Save().Wait();
            }
            
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    var dsf = e as DigitalSignFailure;
                    if (dsf != null)
                    {
                        if (dsf.Win32Code == 2148204547)
                        {
                            // it's what we have is an invalid file, nothing more
                            throw new InvalidFileToSignException();
                        }
                    }
                }
                throw;
            }
        }

        private Binary BinaryLoad(string path)
        {
            LogManager.GetLogger(GetType()).DebugFormat("path is {0}", path);
            return Binary.Load(path, BINARY_LOAD_OPTIONS).Result;
        }
    }
}

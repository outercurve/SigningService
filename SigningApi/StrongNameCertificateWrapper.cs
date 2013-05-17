using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using ClrPlus.Core.Exceptions;
using ClrPlus.Core.Extensions;
using ClrPlus.Windows.Api;

namespace Outercurve.SigningApi
{
    public class StrongNameCertificateWrapper : IDisposable
    {
      
        private string _keyContainer = null;
        public StrongNameCertificateWrapper(X509Certificate2 certificate)
        {
            

            _keyContainer = Guid.NewGuid().ToString();
            var privateKey = (certificate.PrivateKey as RSACryptoServiceProvider).ExportCspBlob(true);

            if (!Mscoree.StrongNameKeyInstall(_keyContainer, privateKey, privateKey.Length))
            {
                throw new ClrPlusException("Unable to create KeyContainer");
            }
        }

        public void Sign(string fileName)
        {
            if (!Mscoree.StrongNameSignatureGeneration(fileName, _keyContainer, IntPtr.Zero, 0, 0, 0))
            {
                throw new ClrPlusException("Unable Strong name assembly '{0}'.".format(fileName));
            }
        }

        public void Dispose()
        {
            if (_keyContainer != null)
            {
                Mscoree.StrongNameKeyDelete(_keyContainer);
                _keyContainer = null;
            }
        }
    }
}

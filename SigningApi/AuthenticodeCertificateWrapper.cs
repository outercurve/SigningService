using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;


using Outercurve.SigningApi.WinApi;
using SigningServiceBase;

namespace Outercurve.SigningApi
{
    public class AuthenticodeCertificateWrapper : ITransientDependency, IDisposable
    {
        private IntPtr _digitalSignInfo = IntPtr.Zero;
        private IntPtr _signContext = IntPtr.Zero;
        private X509Certificate2 _cert = null;

        public AuthenticodeCertificateWrapper(X509Certificate2 certificate)
        {
            _cert = certificate;
        }


        public void Sign(Stream s, string timeStampUrl)
        {
            _signContext = IntPtr.Zero;

            // Prepare signing info: exe and cert
            //
            var digitalSignInfo = new DigitalSignInfo();
            digitalSignInfo.dwSize = Marshal.SizeOf(digitalSignInfo);
            digitalSignInfo.dwSubjectChoice = DigitalSignSubjectChoice.Blob;
        }

        public void Sign(string fileName, string timeStampUrl)
        {
            
            _signContext = IntPtr.Zero;

            // Prepare signing info: exe and cert
            //
            var digitalSignInfo = new DigitalSignInfo();
            digitalSignInfo.dwSize = Marshal.SizeOf(digitalSignInfo);
            digitalSignInfo.dwSubjectChoice = DigitalSignSubjectChoice.File;
            digitalSignInfo.pwszFileName = fileName;
            digitalSignInfo.dwSigningCertChoice = DigitalSigningCertificateChoice.Certificate;
            digitalSignInfo.pSigningCertContext = _cert.Handle;
            digitalSignInfo.pwszTimestampURL = timeStampUrl;

            digitalSignInfo.dwAdditionalCertChoice = DigitalSignAdditionalCertificateChoice.AddChainNoRoot;
            digitalSignInfo.pSignExtInfo = IntPtr.Zero;

         //   var digitalSignExtendedInfo = new DigitalSignExtendedInfo("description", "http://moerinfo");
           // var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf((object) digitalSignExtendedInfo));
            //Marshal.StructureToPtr(digitalSignExtendedInfo, ptr, false);
            // digitalSignInfo.pSignExtInfo = ptr;

            // Sign exe
            //
            
            if ((!CryptUi.CryptUIWizDigitalSign(DigitalSignFlags.NoUI, IntPtr.Zero, null, ref digitalSignInfo, ref _signContext)))
            {
                var rc = (uint)Marshal.GetLastWin32Error();
                if (rc == 0x8007000d)
                {
                    // this is caused when the timestamp server fails; which seems intermittent for any timestamp service.
                   // throw new FailedTimestampException(fileName, timeStampUrl);
                }
              //  throw new DigitalSignFailure(fileName, rc);
            }

        }

       



        public void Dispose()
        {
            if (_signContext != IntPtr.Zero)
            {
                if ((!CryptUi.CryptUIWizFreeDigitalSignContext(_signContext)))
                {
                    //throw new Win32Exception(Marshal.GetLastWin32Error(), "CryptUIWizFreeDigitalSignContext");
                }
            }

            Marshal.FreeCoTaskMem(_digitalSignInfo);
        }
    }
}

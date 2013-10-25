//-----------------------------------------------------------------------
// <copyright company="CoApp Project">
//     Copyright (c) 2010-2013 Garrett Serack and CoApp Contributors. 
//     Contributors can be discovered using the 'git log' command.
//     All rights reserved.
// </copyright>
// <license>
//     The software is licensed under the Apache 2.0 License (the "License")
//     You may not use the software except in compliance with the License. 
// </license>
//-----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace Outercurve.SigningApi.WinApi {
    [StructLayout(LayoutKind.Sequential)]
    public struct DigitalSignInfo {
        public Int32 dwSize;
        public DigitalSignSubjectChoice dwSubjectChoice;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszFileName;

        public DigitalSigningCertificateChoice dwSigningCertChoice;
        public IntPtr pSigningCertContext;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszTimestampURL;

        public DigitalSignAdditionalCertificateChoice dwAdditionalCertChoice;
        public IntPtr pSignExtInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DigitalSignExtendedInfo {
        public uint dwSize;
        public uint dwAttrFlagsNotUsed;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszDescription;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszMoreInfoLocation;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszHashAlg;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszSigningCertDisplayString;

        public IntPtr hAdditionalCertStore;

        public IntPtr psAuthenticated;

        public IntPtr psUnauthenticated;

        public DigitalSignExtendedInfo(string description = null, string moreInfoUrl = null, string hashAlgorithm = null) {
            dwSize = (uint)Marshal.SizeOf(typeof (DigitalSignExtendedInfo));
            dwAttrFlagsNotUsed = 0;
            pwszSigningCertDisplayString = null;
            hAdditionalCertStore = IntPtr.Zero;
            psAuthenticated = IntPtr.Zero;
            psUnauthenticated = IntPtr.Zero;

            pwszDescription = string.IsNullOrEmpty(description) ? null : description;
            pwszMoreInfoLocation = string.IsNullOrEmpty(moreInfoUrl) ? null : moreInfoUrl;
            pszHashAlg = string.IsNullOrEmpty(hashAlgorithm) ? null : hashAlgorithm;
        }
    }

    public struct DigitalSignContext {
        public Int32 dwSize;
        public Int32 cbBlob;
        public IntPtr pbBlob;
    }

    [Flags]
    public enum DigitalSignFlags : uint {
        NoUI = 0x0001,
        ExcludePageHashes = 0x0002,
        IncludePageHashes = 0x0004
    }

    public enum DigitalSignSubjectChoice : uint {
        File = 0x01,
        Blob = 0x02
    }

    public enum DigitalSigningCertificateChoice : uint {
        Certificate = 0x01,
        Store = 0x02,
        PrivateKey = 0x03
    }

    public enum DigitalSignAdditionalCertificateChoice : uint {
        AddChain = 0x0001,
        AddChainNoRoot = 0x0002
    }

    public class CryptUi {
        [DllImport("Cryptui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CryptUIWizDigitalSign(DigitalSignFlags dwFlags, IntPtr hwndParent, string pwszWizardTitle, ref DigitalSignInfo pDigitalSignInfo, ref IntPtr ppSignContext);

        [DllImport("Cryptui.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CryptUIWizFreeDigitalSignContext(IntPtr pSignContext);
    }
}
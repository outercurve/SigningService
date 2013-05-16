using System;
using System.IO;
using System.Linq;

namespace Outercurve.DTO
{
    public static class FileSensing
    {
        public static readonly byte[] PEHEADER = new byte[] { 0x4D, 0x5A};
        public static readonly byte[] ZIPHEADER = new byte[] {0x50, 0x4B};

        public static bool IsItAPEFile(string filePath)
        {
            using (var s = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                return IsItAPEFile(s);
            }
        }

        public static bool IsItAPEFile(Stream s)
        {
            try
            {
                var checkingBytes = new byte[2];
                s.Read(checkingBytes, 0, 2);
                return PEHEADER.SequenceEqual(checkingBytes);
            }
            catch (Exception)
            {
                return false;
                
            }
            
        }

        public static bool IsItAZipFile(string filePath)
        {
            using (var s = File.OpenRead(filePath))
            {
                return IsItAZipFile(s);
            }
        }

        public static bool IsItAZipFile(Stream s)
        {
            try
            {
                var checkingBytes = new byte[2];
                s.Read(checkingBytes, 0, 2);
                return ZIPHEADER.SequenceEqual(checkingBytes);
            }
            catch (Exception)
            {
                return false;

            }
        }
    }
}

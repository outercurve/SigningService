using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using SigningServiceBase.Flags;

namespace Outercurve.SigningApi.WinApi
{
    public class Kernel32
    {
        [DllImport("kernel32.dll", EntryPoint = "MoveFileEx")]
        public static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);

      
    }
}
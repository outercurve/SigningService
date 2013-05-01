using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.Text;
using ClrPlus.Powershell.Core;

namespace Outercurve.Cmdlets.Commands
{
    [Cmdlet(AllVerbs.Assert, "OcfStrongName")]
    public class AssertStrongNameCmdlet :PSCmdlet
    {
        [Parameter(Mandatory = true), ValidateNotNullOrEmptyAttribute]
        public string[] FilePath { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            foreach (var f in GetLocations())
            {
                bool verified = false;
                if (!Mscoree.StrongNameSignatureVerificationEx(f.FullName, true, ref verified))
                {
                    Console.WriteLine("Couldn't attempt to verify {0}");
                    continue;
                }
                if (!verified)
                {
                    Console.WriteLine("{0} is not strong-named.");
                }
            }
            
        }

        private IEnumerable<FileInfo> GetLocations()
        {
            using (var ps = Runspace.DefaultRunspace.Dynamic())
            {
                foreach (var f in FilePath)
                {
                    
                    yield return (ps.GetItem(f) as IEnumerable).OfType<FileInfo>().First();
                }
            }
        }
    }

    public static class Mscoree
    {
        [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
        public static extern bool StrongNameSignatureVerificationEx(string wszFilePath, bool fForceVerification, ref bool pfWasVerified);
    }
}

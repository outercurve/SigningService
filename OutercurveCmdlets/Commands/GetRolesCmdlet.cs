using System;
using System.Management.Automation;
using ClrPlus.Powershell.Core;

namespace Outercurve.Cmdlets.Commands
{
    [Cmdlet(AllVerbs.Get, "OcfRoles")]
    public class GetRolesCmdlet : CredentialedApiCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "Username for the new user")]
        [ValidateNotNullOrEmpty]
        public string Username { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            throw new NotImplementedException();
        }
    }
}

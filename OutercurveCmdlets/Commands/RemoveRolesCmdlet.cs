using System;
using System.Management.Automation;
using ClrPlus.Powershell.Core;

namespace Outercurve.Cmdlets.Commands
{
    [Cmdlet(AllVerbs.Remove, "OcfRoles")]
    public class RemoveRolesCmdlet : CredentialedApiCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true), ValidateNotNullOrEmpty]
        public string UserName { get; set; }

        [Parameter(Position =  1, Mandatory = true), ValidateNotNullOrEmpty]
        public string[] Roles { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            throw new NotImplementedException();
        }
    }
}

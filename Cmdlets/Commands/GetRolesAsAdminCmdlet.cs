using System;
using System.Linq;
using System.Management.Automation;
using ClrPlus.Core.Extensions;
using ClrPlus.Powershell.Core;
using Outercurve.ClientLib.Services;

namespace Outercurve.Cmdlets.Commands
{
    [Cmdlet(AllVerbs.Get, "OcfRolesAsAdmin")]
    public class GetRolesAsAdminCmdlet : CredentialedApiCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "Username to get roles for ")]
        [ValidateNotNullOrEmpty]
        public string Username { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            try
            {
                var userService = new UserService(Credential.UserName, Credential.Password.ToUnsecureString(),
                                                  ServiceUrl, MessageHandler, ProgressHandler);
                WriteObject(userService.GetRolesAsAdmin(Username), true);
            }
            catch (AggregateException ae)
            {
                this.WriteErrorsAndThrowOnLast(ae.Flatten().InnerExceptions.Select(LazyCreateError));
            }
            catch (Exception e)
            {
                ThrowTerminatingError(LazyCreateError(e));
            }
             
        }
    }
}

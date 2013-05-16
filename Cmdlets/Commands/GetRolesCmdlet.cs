using System;
using System.Linq;
using System.Management.Automation;
using ClrPlus.Core.Extensions;
using ClrPlus.Powershell.Core;
using Outercurve.ClientLib.Services;

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
            try
            {
                var userService = new UserService(Credential.UserName, Credential.Password.ToUnsecureString(),
                                                  ServiceUrl, MessageHandler, ProgressHandler);
                userService.GetRoles(Username);
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

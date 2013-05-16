using System;
using System.Linq;
using System.Management.Automation;
using ClrPlus.Core.Extensions;
using ClrPlus.Powershell.Core;
using Outercurve.ToolsLib.Services;

namespace Outercurve.Cmdlets.Commands
{
    [Cmdlet(AllVerbs.Set, "OcfPassword")]
    public class SetPasswordCmdlet : CredentialedApiCmdlet
    {
        [Parameter(Position = 0, Mandatory = true),ValidateNotNullOrEmpty]
        public string NewPassword { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            try
            {
                var userService = new UserService(Credential.UserName, Credential.Password.ToUnsecureString(), ServiceUrl, MessageHandler,
                                                  ProgressHandler);
                userService.SetPassword(NewPassword);
            }
            catch (AggregateException ae)
            {

                this.WriteErrorsAndThrowOnLast(
                    ae.Flatten().InnerExceptions.Select(LazyCreateError));
            }
            catch (Exception e)
            {
                ThrowTerminatingError(LazyCreateError(e));
            }
        }
    }
}

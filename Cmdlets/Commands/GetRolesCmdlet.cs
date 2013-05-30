using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using ClrPlus.Core.Extensions;
using ClrPlus.Powershell.Core;
using Outercurve.ClientLib.Services;

namespace Outercurve.Cmdlets.Commands
{
    [Cmdlet(AllVerbs.Get, "OcfRoles")]
    public class GetRolesCmdlet : CredentialedApiCmdlet
    {
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            try
            {
                var userService = new UserService(Credential.UserName, Credential.Password.ToUnsecureString(),
                                                  ServiceUrl, MessageHandler, ProgressHandler);
                WriteObject(userService.GetRoles(), true);
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

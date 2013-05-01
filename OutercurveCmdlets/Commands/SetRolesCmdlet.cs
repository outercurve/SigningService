using System;
using System.Linq;
using System.Management.Automation;
using ClrPlus.Core.Extensions;
using ClrPlus.Powershell.Core;
using Outercurve.ToolsLib.Services;

namespace Outercurve.Cmdlets.Commands
{
    [Cmdlet(AllVerbs.Set, "OcfRoles")]
    public class SetRolesCmdlet : CredentialedApiCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true), ValidateNotNullOrEmpty]
        public string UserName { get; set; }

        [Parameter(Position = 1, Mandatory = true), ValidateNotNullOrEmpty]
        public string[] Roles { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            try
            {
                var userService = new UserService(Credential.UserName, Credential.Password.ToUnsecureString(), ServiceUrl, MessageHandler,
                                                  ProgressHandler);
                userService.SetRoles(UserName, Roles);
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

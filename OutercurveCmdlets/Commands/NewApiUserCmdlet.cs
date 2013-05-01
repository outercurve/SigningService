using System;
using System.Linq;
using System.Management.Automation;
using ClrPlus.Core.Extensions;
using ClrPlus.Powershell.Core;
using Outercurve.ToolsLib.Services;

namespace Outercurve.Cmdlets.Commands
{
    [Cmdlet(AllVerbs.New, "OcfUser")]
    public class NewApiUserCmdlet : CredentialedApiCmdlet   
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "Username for the new user")]
        [ValidateNotNullOrEmpty]
        public string Username { get; set; }

        [Parameter(Position = 1, HelpMessage = "Password for the new user. If none is specified, an autocreated password will be returned. ")]
        public string Password { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            try
            {
                var userCreator = new UserService(Credential.UserName, Credential.Password.ToUnsecureString(), ServiceUrl, MessageHandler, ProgressHandler);
                
                if (String.IsNullOrEmpty(Password))
                {
                    WriteObject(userCreator.CreateUser(Username));
                }
                else
                {
                    userCreator.CreateUserWithPassword(Username, Password);
                }
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

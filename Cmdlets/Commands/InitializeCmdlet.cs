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
    [Cmdlet(AllVerbs.Initialize, "OcfSystem")]
    public class InitializeSystemCmdlet : ApiCmdlet
    {
        [Parameter(Mandatory = true), ValidateNotNullOrEmptyAttribute]
        public string UserName { get; set; }
        
        [Parameter(Mandatory = true), ValidateNotNullOrEmptyAttribute]
        public string Password { get; set; }

        protected override void ProcessRecord()
        {
           
            base.ProcessRecord();
            try
            {
                var userService = new UserService(ServiceUrl, MessageHandler,
                                                  ProgressHandler);
                userService.Initialize(UserName, Password);
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

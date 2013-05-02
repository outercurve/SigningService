using System.Management.Automation;
using ClrPlus.Core.Configuration;
using ClrPlus.Core.Extensions;
using Outercurve.ToolsLib.Services;

namespace Outercurve.Cmdlets.Commands
{
    [Cmdlet(VerbsCommon.Set, "OcfDefaultRemoteService")]
    public class SetDefaultRemoteServiceCmdlet : Cmdlet
    {
        private readonly IDefaultCredentialService _credentialsService;

        public SetDefaultRemoteServiceCmdlet()
        {
            _credentialsService = new DefaultCredentialService();
        }

        internal SetDefaultRemoteServiceCmdlet(IDefaultCredentialService service)
        {
            _credentialsService = service;
        }

        [Parameter(HelpMessage = "Remote Service URL")]
        public string ServiceUrl { get; set; }

        [Parameter(HelpMessage = "Credentials to user for remote service")]
        public PSCredential Credential { get; set; }

        [Parameter(HelpMessage = "Remove any defaults stored in the user registry")]
        public SwitchParameter Forget { get; set; }


        protected override void ProcessRecord()
        {
            if (Forget)
            {
                _credentialsService.Forget();
                return;
            }
           
            if (Credential != null)
            {
                _credentialsService.Set(ServiceUrl, Credential.UserName, Credential.Password.ToUnsecureString());   
            }
            else
            {
                _credentialsService.Set(ServiceUrl);
            }
        }
    }
}

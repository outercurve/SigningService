using System.Management.Automation;
using ClrPlus.Core.Extensions;

namespace Outercurve.Cmdlets.Commands
{
    public abstract class CredentialedApiCmdlet : ApiCmdlet
    {

        [Parameter]
        public PSCredential Credential { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();


            if (Credential == null)
            {
                var cred = CredentialService.GetCredential();
                Credential = new PSCredential(cred.UserName, cred.Password.ToSecureString());
            }

            if (Credential == null)
            {
                ThrowTerminatingError(new ErrorRecord(new ParameterBindingException("Credential was null"), "",
                                                      ErrorCategory.InvalidArgument, null));
            }
        }
    }
}
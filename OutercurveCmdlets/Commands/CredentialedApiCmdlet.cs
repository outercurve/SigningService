using System.Management.Automation;

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
                Credential = SetDefaultRemoteServiceCmdlet.DefaultCredential;
            }

            if (Credential == null)
            {
                ThrowTerminatingError(new ErrorRecord(new ParameterBindingException("Credential was null"), "",
                                                      ErrorCategory.InvalidArgument, null));
            }
        }
    }
}
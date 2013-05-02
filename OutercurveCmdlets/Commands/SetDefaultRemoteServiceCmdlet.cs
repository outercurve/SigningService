using System.Management.Automation;
using ClrPlus.Core.Configuration;
using ClrPlus.Core.Extensions;

namespace Outercurve.Cmdlets.Commands
{
    [Cmdlet(VerbsCommon.Set, "OcfDefaultRemoteService")]
    public class SetDefaultRemoteServiceCmdlet : Cmdlet
    {
        private static string _defaultServiceUrl;
        private static PSCredential _defaultCredential;

        [Parameter(HelpMessage = "Remote Service URL")]
        public string ServiceUrl { get; set; }

        [Parameter(HelpMessage = "Credentials to user for remote service")]
        public PSCredential Credential { get; set; }

        [Parameter(HelpMessage = "Remove any defaults stored in the user registry")]
        public SwitchParameter Forget { get; set; }

        public static string DefaultServiceUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultServiceUrl))
                {
                    return RegistryView.User[@"Software\Outercurve\RemoteCmdlet#DefaultServiceUrl"].EncryptedStringValue;
                }
                return _defaultServiceUrl;
            }
            set
            {
                _defaultServiceUrl = value;
            }
        }

        public static PSCredential DefaultCredential
        {
            get
            {
                if (_defaultCredential == null)
                {
                    var user = RegistryView.User[@"Software\Outercurve\RemoteCmdlet#DefaultUser"].EncryptedStringValue;
                    var pass = RegistryView.User[@"Software\Outercurve\RemoteCmdlet#DefaultPassword"].EncryptedStringValue;
                    return string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) ? null : new PSCredential(user, pass.ToSecureString());
                }
                return _defaultCredential;
            }
            set
            {
                _defaultCredential = value;
            }
        }

        protected override void ProcessRecord()
        {
            if (Forget)
            {
                DefaultServiceUrl = null;
                DefaultCredential = null;
                RegistryView.User[@"Software\ClrPlus\RemoteCmdlet"].DeleteValues();
                return;
            }

            DefaultServiceUrl = ServiceUrl;
            DefaultCredential = Credential;

            
            RegistryView.User[@"Software\ClrPlus\RemoteCmdlet"].DeleteValues();

            RegistryView.User[@"Software\ClrPlus\RemoteCmdlet#DefaultServiceUrl"].EncryptedStringValue = ServiceUrl;
            if (Credential != null)
            {
                RegistryView.User[@"Software\ClrPlus\RemoteCmdlet#DefaultUser"].EncryptedStringValue = Credential.UserName;
                RegistryView.User[@"Software\ClrPlus\RemoteCmdlet#DefaultPassword"].EncryptedStringValue = Credential.Password.ToUnsecureString();
            }
            
        }
    }
}

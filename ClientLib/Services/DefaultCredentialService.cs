using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClrPlus.Core.Configuration;
using ClrPlus.Core.Extensions;

namespace Outercurve.ToolsLib.Services
{
    
    public class DefaultCredentialService : IDefaultCredentialService
    {
        private const string REGISTRY_KEY_NAME = "Outercurve";

        public string GetUri()
        {
            return RegistryView.User[@"Software\{0}\RemoteCmdlet#DefaultServiceUrl".format(REGISTRY_KEY_NAME)].EncryptedStringValue;
        }

        public Credential GetCredential()
        {
            var user = RegistryView.User[@"Software\{0}\RemoteCmdlet#DefaultUser".format(REGISTRY_KEY_NAME)].EncryptedStringValue;
            var pass = RegistryView.User[@"Software\{0}\RemoteCmdlet#DefaultPassword".format(REGISTRY_KEY_NAME)].EncryptedStringValue;
            return string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass)
                       ? null
                       : new Credential(user, pass);
        }

        public void Set(string uri)
        {
            Set(uri, null);
        }

        public void Set(string uri, string userName, string password)
        {

            Set(uri, new Credential(userName, password));
        }

        public void Set(string uri, Credential credential)
        {
            RegistryView.User[@"Software\{0}\RemoteCmdlet".format(REGISTRY_KEY_NAME)].DeleteValues();

            RegistryView.User[@"Software\{0}\RemoteCmdlet#DefaultServiceUrl".format(REGISTRY_KEY_NAME)]
                .EncryptedStringValue = uri;

            if (credential != null)
            {
                RegistryView.User[@"Software\{0}\RemoteCmdlet#DefaultUser".format(REGISTRY_KEY_NAME)]
                    .EncryptedStringValue = credential.UserName;
                RegistryView.User[@"Software\{0}\RemoteCmdlet#DefaultPassword".format(REGISTRY_KEY_NAME)]
                    .EncryptedStringValue = credential.Password;
            }
        }

        public void Forget()
        {
            RegistryView.User[@"Software\{0}\RemoteCmdlet".format(REGISTRY_KEY_NAME)].DeleteValues();
        }
    }



    public interface IDefaultCredentialService
    {
        string GetUri();
        Credential GetCredential();
        void Set(string uri);
        void Set(string uri, string userName, string password);
        void Set(string uri, Credential credential);
        void Forget();
    }

    public class Credential
    {
        public Credential(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public string UserName { get; private set; }
        public string Password { get; private set; }
        
    }
}

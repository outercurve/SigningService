using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using SigningServiceBase;

namespace Outercurve.HttpCredentials
{
    public class HttpCredentialsModule : Module
    {
        public string BaseUrl { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpCredentialsStore>()
                .WithParameter(new NamedParameter("baseUrl", BaseUrl))
                .As<ISimpleCredentialStore>();
        }
    }
}

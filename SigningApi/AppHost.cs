using Funq;
using Outercurve.Api;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;

namespace Outercurve.SigningApi
{
    public class AppHost : AppHostBase

    {
        public static readonly string ServiceName = "Outercurve Api";

        public AppHost() : base(ServiceName, typeof(AppHost).Assembly)
        {

        }

        public override void Configure(Container container)
        {
            container.Register(this);
            LogManager.LogFactory = new NLogFactory();
            container.RegisterAutoWired<AppSettings>();
            container.RegisterAutoWired<FsService>();
            container.RegisterAutoWired<AzureClient>();
            container.RegisterAutoWired<CertService>();
            container.RegisterAutoWired<CustomBasicAuthProvider>();
            container.Register(LogManager.GetLogger(GetType()));
            container.RegisterAutoWired<LoggingService>();
            Plugins.Add(new AuthFeature(() => new AuthUserSession(), new IAuthProvider[] { container.Resolve<CustomBasicAuthProvider>() }) { HtmlRedirect = null, IncludeAssignRoleServices = false});
         

        }

        
    }
}
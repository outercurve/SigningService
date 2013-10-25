using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Autofac;
using Autofac.Core.Lifetime;
using Autofac.Integration.Mvc;
using Funq;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;
using SigningServiceBase;

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
            //container.Register(this);
            var builder = new ContainerBuilder();
            //register the default Module so we can load stuff from extensions
            builder.RegisterModule<DefaultModule>();

            var outerContainer = builder.Build();
            var innerContainerBuilder = new ContainerBuilder();

            CopyFromOneToAnother(outerContainer, innerContainerBuilder);
            RegisterExtensions(outerContainer, innerContainerBuilder);
            RegisterAssemblyTypes(Assembly.GetExecutingAssembly(), innerContainerBuilder);

            
            LogManager.LogFactory = new NLogFactory();
         

            
            
            IContainerAdapter adapter = new AutofacIocAdapter(innerContainerBuilder.Build());
            container.Adapter = adapter;
            
            Plugins.Add(new AuthFeature(() => new AuthUserSession(), new IAuthProvider[] { container.Resolve<ISimpleCredentialStore>() }) { HtmlRedirect = null, IncludeAssignRoleServices = false });

        }




        private void RegisterAssemblyTypes(Assembly a, ContainerBuilder builder)
        {
            Type iDependencyType = typeof (IDependency);
            builder.RegisterAssemblyTypes(a).Where(t => t.GetInterfaces().Contains(iDependencyType));
        }

        private void RegisterExtensions(IContainer outerContainer, ContainerBuilder builder)
        {
            var loader = outerContainer.Resolve<IModuleLoader>();
            foreach (var assembly in loader.GetAssembliesInModules())
            {
                RegisterAssemblyTypes(assembly, builder);
            }
        }


        public void CopyFromOneToAnother(IContainer outer, ContainerBuilder builder)
        {
            var components = outer.ComponentRegistry.Registrations
                    .Where(cr => cr.Activator.LimitType != typeof(LifetimeScope));

            foreach (var c in components)
            {
                builder.RegisterComponent(c);
            }

            foreach (var source in outer.ComponentRegistry.Sources)
            {
                builder.RegisterSource(source);
            }
        }
    }
}
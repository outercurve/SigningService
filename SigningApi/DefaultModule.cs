using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.Mvc;
using SigningServiceBase;

namespace Outercurve.SigningApi
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystem>().As<IFileSystem>();
            builder.RegisterModule(new AutofacWebTypesModule());
            builder.RegisterType<DefaultModulesLoader>().As<IModuleLoader>();
        }
    }
}
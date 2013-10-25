using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Web;
using SigningServiceBase;

namespace Outercurve.SigningApi
{
    public class DefaultModulesLoader : IModuleLoader
    {
        private readonly IFileSystem _fileSystem;
        private readonly HttpServerUtilityBase _serverUtility;

        public DefaultModulesLoader(IFileSystem fileSystem, HttpServerUtilityBase serverUtility)
        {
            _fileSystem = fileSystem;
            _serverUtility = serverUtility;
        }

        public IEnumerable<Assembly> GetAssembliesInModules()
        {
            var possibleAssemblies = _fileSystem.DirectoryInfo
                .FromDirectoryName(_serverUtility.MapPath("~/Modules"))
                .GetFiles("*.dll", SearchOption.AllDirectories);
            foreach (var possibleAssm in  possibleAssemblies)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFile(possibleAssm.FullName);
                }
                catch
                {
                    //nothing
                }

                if (assembly != null)
                    yield return assembly;

            }
        }
    }
}
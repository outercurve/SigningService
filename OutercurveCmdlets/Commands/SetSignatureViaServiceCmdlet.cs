using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using ClrPlus.Core.Extensions;
using ClrPlus.Powershell.Core;
using Outercurve.ToolsLib;
using Outercurve.ToolsLib.Services;

namespace Outercurve.Cmdlets.Commands
{
    [Cmdlet(VerbsCommon.Set, "OcfSignatureViaService")]
    public class SetSignatureViaServiceCmdlet : CredentialedApiCmdlet
    {

        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public string[] FilePath { get; set; }

        [Parameter(Position = 1)]
        public string OutputPath { get; set; }

        [Parameter]
        public SwitchParameter StrongName { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            try
            {
                var sourcesToDestination = MapSourceToDestination(GetFiles());
                var signer = new SigningService(Credential.UserName, Credential.Password.ToUnsecureString(),
                                        sourcesToDestination, ServiceUrl,
                                        MessageHandler, ProgressHandler);
                signer.Sign(StrongName);
            }
            catch (AggregateException ae)
            {

                this.WriteErrorsAndThrowOnLast(
                    ae.Flatten().InnerExceptions.Select(LazyCreateError));
            }
            catch (Exception e)
            {
                ThrowTerminatingError(LazyCreateError(e));
            }
        }

        private IEnumerable<SourceToDestinationMap<FileInfo>> MapSourceToDestination(IEnumerable<FileInfo> inputs)
        {
            var destination = ResolveDestination();
            if (destination is FileInfo)
            {
                var fileInfos = inputs as FileInfo[] ?? inputs.ToArray();
                if (fileInfos.Count() > 1)
                  {
                      ThrowTerminatingError(new ErrorRecord(new DirectoryNotFoundException(), "0", ErrorCategory.InvalidArgument, null));
                  }

                yield return new SourceToDestinationMap<FileInfo> {Source = fileInfos.First(), Destination = destination as FileInfo};
                yield break;
            }
            
            //it's a Directory!
           
            foreach (var i in inputs)
            {
                yield return new SourceToDestinationMap<FileInfo> { Source = i, Destination = new FileInfo(Path.Combine(destination.FullName, i.Name)) };
            }
           
        }


        private FileSystemInfo ResolveDestination()
        {
            using (var ps = Runspace.DefaultRunspace.Dynamic())
            {
                DynamicPowershellResult output = ps.GetItem(OutputPath ?? FilePath[0]);
                if (output.Errors.Any())
                {
                    if (output.Errors.Any(e => e.Exception is ItemNotFoundException))
                    {
                        //the destination didn't exist, probably a file
                        var lastSlash = OutputPath.LastIndexOf('\\');
                        var hasASlash = lastSlash >= 0;
                        var probablyDirectoryDestination = hasASlash ? OutputPath.Substring(0, lastSlash) : ".";

                        output = ps.GetItem(probablyDirectoryDestination);
                        if (output.Errors.Any())
                        {
                            var ex = output.Errors.FirstOrDefault(e => e.Exception is ItemNotFoundException);
                            this.WriteErrors(output.Errors.Where(e => e != ex));

                            ThrowTerminatingError(new ErrorRecord(new Exception("{0} does not exist, nor does {1}".format(OutputPath, probablyDirectoryDestination), ex.Exception), "", ErrorCategory.InvalidData, null));
                           
                        }

                        if (output.Count != 1)
                        {
                           ThrowTerminatingError(new ErrorRecord(new Exception("OutputPath may not be a wildcard"), "", ErrorCategory.InvalidData, null ));
                        }

                        var dir = output.First() as FileSystemInfo;

                        return
                            new FileInfo(Path.Combine(dir.FullName,
                                                      hasASlash ? OutputPath.Substring(lastSlash + 1) : OutputPath));
                                                      
                    }

                    this.WriteErrorsAndThrowOnLast(output.Errors);
                }
                
                if (output.Count != 1)
                {
                    ThrowTerminatingError(new ErrorRecord(new Exception("OutputPath may not be a wildcard"), "", ErrorCategory.InvalidData, null ));
                }

                return output.First() as FileSystemInfo;
            }
        }


          

        private IEnumerable<FileInfo> GetFiles()
        {
            using (var ps = Runspace.DefaultRunspace.Dynamic())
            {
                dynamic _ps = ps;
                return FilePath.SelectMany<string,FileInfo>(f => Enumerable.OfType<FileInfo>(_ps.GetItem(f)));
            }
        }

    }
}

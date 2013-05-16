using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Outercurve.ClientLib;
using Outercurve.ClientLib.Messages;
using Outercurve.ClientLib.Services;

namespace Outercurve.MSBuildTasks
{
    public class SetSignature : Task
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDefaultCredentialService _credentialService;

        public SetSignature(): this(new FileSystem(), new DefaultCredentialService()) 
        {
            
        }
       

        internal SetSignature(IFileSystem fileSystem, IDefaultCredentialService credentialService)
        {
            _fileSystem = fileSystem;
            _credentialService = credentialService;
        }
       

        [Required]
        public ITaskItem[] InputFiles { get; set; }

        [Required]

        public string OutputDir { get; set; }

        
        public bool StrongName { get; set; }
        public string ServiceUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        
        public override bool Execute()
        {
            try
            {
                Log.LogMessage("Something");
                SetCredentials();
                var sourcesToDestination = MapSourcesToDestination();
                Log.LogMessage(sourcesToDestination.ToArray().First().Destination.FullName);

                var signer = new SigningService(UserName, Password,
                                        sourcesToDestination, ServiceUrl,
                                        MessageHandler, ProgressHandler);
                signer.Sign(StrongName);
                
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e, true, true, null);
            }

            return !Log.HasLoggedErrors;

        }

        private void ProgressHandler(ProgressMessage progressMessage)
        {
            
           // WriteProgress(new ProgressRecord(progressMessage.ActivityId, progressMessage.Activity, progressMessage.Description) { PercentComplete = progressMessage.PercentComplete, RecordType = progressMessage.MessageType.CastToString().CastToEnum<ProgressRecordType>() });
        }

        private void MessageHandler(Message message)
        {
            switch (message.MessageType)
            {

                case MessageType.Info: 
                    Log.LogMessage(MessageImportance.Normal, message.Contents);
                    break;
                case MessageType.Warning: 
                    Log.LogWarning(message.Contents);

                    break;
            }
        }

        private IEnumerable<SourceToDestinationMap<FileInfoBase>> MapSourcesToDestination()
        {
            
            var fullOutputDir = _fileSystem.Path.GetFullPath(OutputDir);
            var alloutputs = InputFiles.Select(i => Path.Combine(fullOutputDir.TrimEnd('/','\\'), i.ItemSpec.Trim('/','\\')));
            var something  = alloutputs.ToArray();
                
            var allOutputs = something.Select(s => _fileSystem.FileInfo.FromFileName(s)).ToArray();
            return allOutputs.Select(i => new SourceToDestinationMap<FileInfoBase> {Source = i, Destination = i});
           

        }

        private void SetCredentials()
        {
            if (String.IsNullOrEmpty(UserName) || String.IsNullOrEmpty(Password) || String.IsNullOrEmpty(ServiceUrl))
            {
                var cred = _credentialService.GetCredential();
                UserName = cred.UserName;
                Password = cred.Password;
                ServiceUrl = _credentialService.GetUri();
            }

            var exceptions = new List<Exception>();
            if (String.IsNullOrEmpty(UserName))
            {
                exceptions.Add(new Exception(@"UserName wasn't set. Either add it to your project/msbuild file or run Set-OcfDefaultRemoteService"));
            }
            if (String.IsNullOrEmpty(Password))
            {
                exceptions.Add(new ArgumentException(@"Password wasn't set. Either add it to your project/msbuild file or run Set-OcfDefaultRemoteService"));
            }

            if (String.IsNullOrEmpty(ServiceUrl))
            {
                exceptions.Add(new ArgumentException(@"ServiceUrl wasn't set. Either add it to your project/msbuild file or run Set-OcfDefaultRemoteService"));
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
            
           
        }
    }
}

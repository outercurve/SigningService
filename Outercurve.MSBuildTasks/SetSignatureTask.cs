using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Outercurve.ToolsLib;
using Outercurve.ToolsLib.Messages;
using Outercurve.ToolsLib.Services;

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

                SetCredentials();
                var sourcesToDestination = MapSourcesToDestination();

                var signer = new SigningService(UserName, Password,
                                        sourcesToDestination, ServiceUrl,
                                        MessageHandler, ProgressHandler);
                signer.Sign(StrongName);
                return true;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e, true, true, null);
            }

            return Log.HasLoggedErrors;

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
                    Log.LogMessage(MessageImportance.Low, message.Contents);
                    break;
                case MessageType.Warning: 
                    Log.LogWarning(message.Contents);

                    break;
            }
        }

        private IEnumerable<SourceToDestinationMap<FileInfoBase>> MapSourcesToDestination()
        {
            
            var fullOutputDir = _fileSystem.Path.GetFullPath(OutputDir);
            var alloutputs = InputFiles.Select(i => _fileSystem.Path.Combine(fullOutputDir, i.ItemSpec)).Select(s => _fileSystem.FileInfo.FromFileName(s));
            return alloutputs.Select(i => new SourceToDestinationMap<FileInfoBase> {Source = i, Destination = i});
           

        }

        private void SetCredentials()
        {
            if (String.IsNullOrEmpty(UserName) || String.IsNullOrEmpty(Password))
            {
                var cred = _credentialService.GetCredential();
                UserName = cred.UserName;
                Password = cred.Password;
            }

            var exceptions = new List<Exception>();
            if (String.IsNullOrEmpty(UserName))
            {
               exceptions.Add(new ArgumentException("UserName"));
            }
            if (String.IsNullOrEmpty(Password))
            {
                exceptions.Add(new ArgumentException("Password"));
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
            
           
        }
    }
}

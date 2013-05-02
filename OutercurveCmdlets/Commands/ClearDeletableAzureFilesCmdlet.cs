using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using ClrPlus.Core.Extensions;
using ClrPlus.Powershell.Core;
using Outercurve.DTOs.Services.Azure;
using Outercurve.ToolsLib.Messages;
using Outercurve.ToolsLib.Services;

namespace Outercurve.Cmdlets.Commands
{
    [Cmdlet(AllVerbs.Clear, "DeletableAzureFiles")]
    public class ClearDeletableAzureFilesCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0), ValidateNotNullOrEmpty]
        public string AzureAccount { get; set; }

        [Parameter(Mandatory = true, Position = 1), ValidateNotNullOrEmpty]
        public string AzurePassword { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            var files = new AzureFilesService(new AzureService(AzureAccount, AzurePassword), MessageHandler, ProgressHandler);
            files.DeleteFiles();
            
        }

        protected void ProgressHandler(ProgressMessage progressMessage)
        {
            WriteProgress(new ProgressRecord(progressMessage.ActivityId, progressMessage.Activity, progressMessage.Description) { PercentComplete = progressMessage.PercentComplete, RecordType = progressMessage.MessageType.CastToString().CastToEnum<ProgressRecordType>() });
        }

        protected void MessageHandler(Message message)
        {
            switch (message.MessageType)
            {
                case MessageType.Info: Console.WriteLine(message.Contents);
                    break;
                case MessageType.Warning: WriteWarning(message.Contents);
                    break;

            }
        }
    }
}

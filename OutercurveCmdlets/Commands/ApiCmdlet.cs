using System;
using System.Management.Automation;
using ClrPlus.Core.Extensions;
using Outercurve.ToolsLib.Messages;

namespace Outercurve.Cmdlets.Commands
{
    public abstract class ApiCmdlet : PSCmdlet
    {

        [Parameter]
        public string ServiceUrl { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            
            if (String.IsNullOrWhiteSpace(ServiceUrl))
            {
                ServiceUrl = SetDefaultRemoteServiceCmdlet.DefaultServiceUrl;
            }

            if (String.IsNullOrWhiteSpace(ServiceUrl))
            {
                ThrowTerminatingError(new ErrorRecord(new ParameterBindingException("ServiceUrl was null or empty"), "", ErrorCategory.InvalidArgument, null)); 
            }
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

        protected ErrorRecord LazyCreateError(Exception e)
        {
            return new ErrorRecord(e, "", ErrorCategory.NotSpecified, null);
        }
    }
}

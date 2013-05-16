using System;
using System.Collections.Generic;
using System.Linq;
using ClrPlus.Core.Extensions;
using Outercurve.DTOs.Services.Azure;
using Outercurve.ToolsLib.Messages;

namespace Outercurve.ToolsLib.Services
{
    public class AzureFilesService
    {
        protected Action<Message> SendMessage;
        protected Action<ProgressMessage> SendProgress;
        private readonly IAzureService _service;


        public AzureFilesService(IAzureService service, Action<Message> messageHandler = null,
                          Action<ProgressMessage> progressHandler =
                              null)
        {
            _service = service;
            SendMessage = messageHandler ?? (m => { });
            SendProgress = progressHandler ?? (m => { });
        }

        public void DeleteFiles()
        {

            var deletableContainers = (from c in _service.Containers
                                      let sap = c.GetSharedAccessPolicies()
                                      where c.Name.StartsWith("deletable")
                                      where sap.ContainsKey("mypolicy")
                                      where sap["mypolicy"].SharedAccessExpiry < DateTimeOffset.UtcNow
                                      select c).ToArray();

            


            for (int i= 0; i < deletableContainers.Length; i++)
            {
               var d = deletableContainers[i];
                SendProgress(new ProgressMessage
                    {
                        Activity = "Deleting {0} of {1}".format(i, deletableContainers.Length),
                        ActivityId = 1,
                        Description = "Deleting {0}".format(d.Uri.ToString()),
                        MessageType = ProgressMessageType.Processing,
                        PercentComplete = (int)(((double)i/deletableContainers.Length)*100)
                    });
                    d.Delete();
            }
        }

        protected void ThrowErrors(IEnumerable<string> errors)
        {

            var errArray = (errors ?? Enumerable.Empty<string>()).ToArray();

            if (!errArray.Any())
            {
                return;
            }
            if (errArray.Length == 1)
            {
                throw new Exception(errArray[0]);
            }
            else
            {
                throw new AggregateException(errArray.Select(i => new Exception(i)));
            }

        }

    }
}

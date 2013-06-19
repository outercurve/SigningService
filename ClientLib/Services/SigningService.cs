using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClrPlus.Core.Extensions;
using Outercurve.ClientLib.IoItem;
using Outercurve.ClientLib.Messages;
using Outercurve.DTO.Request;
using Outercurve.DTO.Response;
using Outercurve.DTO.Services.Azure;

namespace Outercurve.ClientLib.Services
{
    public class SigningService : Service
    {
        
        private IAzureService _service;
       
        private readonly IEnumerable<SourceToDestinationMap<IIoItem>> _files;

        public SigningService(string username, string password, IEnumerable<SourceToDestinationMap<FileInfoBase>> sourcesToDestinations, string serviceUrl, Action<Message> messageHandler = null, Action<ProgressMessage> progressHandler = null)
            : base(username, password, serviceUrl,messageHandler, progressHandler)
        {
            IEnumerable<SourceToDestinationMap<FileInfoBase>> sourceToDestinationMaps = sourcesToDestinations as SourceToDestinationMap<FileInfoBase>[] ?? sourcesToDestinations.ToArray();
            if (sourceToDestinationMaps.IsNullOrEmpty())
            {
                throw new ArgumentException("", "sourcesToDestinations");
            }

            _files =
              sourceToDestinationMaps.Select(
                  fsInfoMap =>
                  new SourceToDestinationMap<IIoItem>
                  {
                      Source = IoItemFactory.Create(fsInfoMap.Source),
                      Destination = IoItemFactory.Create(fsInfoMap.Destination)
                  }).ToArray();
        }

       


        private void GetAzureService(string account, string sas, string uri)
        {
            
            var azure = uri.StartsWith("http://127.0.0.1:10000") ? AzureService.CreateDeveloperSasService(account, sas) : AzureService.CreateSasService(account, sas);
            
            _service = azure;
        }

        /// <summary>
        /// warning
        /// </summary>
        /// <returns>message if there was a terminating error, else null</returns>
        public void Sign(bool strongName, CancellationTokenSource tokenSource = default(CancellationTokenSource))
        {
            var t = new Task(() =>
                {
                    var response = Client.Post(new GetUploadLocationRequest());
                    
                    GetAzureService(response.Account, response.Sas, response.Location);
                    var location = _service.GetContainer(response.Name);
                    foreach (var f in _files)
                    {
                        if (tokenSource != null && tokenSource.IsCancellationRequested)
                        {
                            break;
                        }
                        SignAFile(location, f, strongName, tokenSource);

                    }
                });

            t.RunSynchronously();

            t.Wait();

        }



        private TimeSpan CopyTo(IIoItem source, IIoItem destination)
        {
            SendMessage(new Message
                                {
                                    Contents = "Starting Copy '{0}' to '{1}'".format(source.FullName,
                                                                                     destination
                                                                                         .FullName),
                                    MessageType = MessageType.Info
                                });
            var startTime = DateTime.Now;
            using (var from = source.OpenRead())
                {
                using (var to = destination.OpenWrite())
                {
                    
                

                    from.CopyTo(to);
                    var inputLength = from.Length;
                   /* to.BytesWritten += (sender, args) => SendProgress (new ProgressMessage
                                                             {
                                                                 Activity =  "Copy",
                                                                 ActivityId = 1,
                                                                 Description =
                                                                     "Copying '{0}' to '{1}'".format(source.FullName,
                                                                                                     destination
                                                                                                         .FullName),
                                                                 PercentComplete =
                                                                     (int)
                                                                     (100*(double) args.StreamPosition/inputLength)
                                                             });



                    var t = from.CopyToAsync(to, new CancellationToken(), false);
                    
                        t.RunSynchronously();
                        SendProgress(new ProgressMessage
                                             {
                                                 Activity =  "Copy",
                                                 ActivityId = 1,
                                                 Description =
                                                     "Copying '{0}' to '{1}'".format(source.FullName,
                                                                                     destination
                                                                                         .FullName),
                                                 PercentComplete =
                                                     100,
                                                 MessageType = ProgressMessageType.Complete
                                             });
                    

                    SendMessage(new Message
                                {
                                    Contents = "Finished Copy",
                                    MessageType = MessageType.Info
                                });
                        }
                        );

                    */
                    /*
                    t.RunSynchronously();


                    t.Wait();*/

                }
            }
            var endTime = DateTime.Now;
            return endTime - startTime;
        }

        private void SignAFile(IAzureContainer container, SourceToDestinationMap<IIoItem> file, bool strongName, CancellationTokenSource tokenSource)
        {

            var blob = IoItemFactory.Create(container.GetBlob(file.Source.Name.ToLower()));
            var copyToTime = CopyTo(file.Source, blob);

            if (tokenSource != null && tokenSource.IsCancellationRequested)
            {
                return;
            }

            SendMessage(new Message
                       {
                           Contents = "Starting signing for {0}".format(file.Destination.FullName),
                           MessageType = MessageType.Info
                       });
           
            var r = Client.Post(new SetCodeSignatureRequest
            {
                Container = container.Name,
                Path = blob.Name,
                StrongName = strongName
            });

            ThrowErrors(r.Errors);

            var statusResult = KeepGettingStatus(container.Name, blob.Name, copyToTime, tokenSource);
            if (statusResult.TimedOut)
            {
                throw new TimeoutException("The signing job has timed out and probably failed. If you'd like to check the status in the future for this job," +
                                            "you can call get-status again with Container={0} and Path={1}".format(container.Name, blob.Name));
            }

            if (statusResult.Exception != null)
            {
                throw statusResult.Exception;
            }

            if (statusResult.Status == StatusCode.Failed)
            {
                throw new Exception("The signing job for {0}/{1} has failed.".format(container.Name, blob.Name));
            }



            if (tokenSource != null && tokenSource.IsCancellationRequested)
            {
                return;
            }
            

            SendMessage(new Message
            {
                Contents = "Finished signing for {0}".format(file.Destination.FullName),
                MessageType = MessageType.Info
            });

            
            CopyTo(blob, file.Destination);
        }

        private StatusResult KeepGettingStatus(string container, string blob, TimeSpan timeToUpload, CancellationTokenSource tokenSource)
        {
            
            try
            {
                StatusCode c = StatusCode.WaitingToRun;
                 SendMessage(new Message
                        {
                            Contents = @"We're waiting for signing to run on {0}\{1}".format(container, blob)
                        });
                var getStatusMessage = new GetStatus {Container = container, Path = blob};
                while (c == StatusCode.WaitingToRun)
                {
                    if (tokenSource != null && tokenSource.IsCancellationRequested)
                    {
                        return null;
                    }
                    Thread.Sleep(3000);
                    var r = Client.Post(getStatusMessage);
                    ThrowErrors(r.Errors);
                    c = r.Status;
                }
                
                SendMessage(new Message
                {
                    Contents = @"Signing has started for {0}\{1}".format(container, blob)
                });

                var startSigningTime = DateTime.Now;
                
                // we wait three times as long as timeToUpload
                while (c == StatusCode.Running)
                {
                    if (tokenSource != null && tokenSource.IsCancellationRequested)
                    {
                        return null;
                    }

                    if ((DateTime.Now - startSigningTime) > new TimeSpan(timeToUpload.Ticks*3))
                    {
                        return new StatusResult {TimedOut = true};
                    }
                    var r = Client.Post(getStatusMessage);
                    ThrowErrors(r.Errors);
                    c = r.Status;

                }

                return new StatusResult {Status = c};

                

            }
            catch (Exception e)
            {
                return new StatusResult {Exception = e};
            }
            
           
        }


        public class StatusResult
        {
            public StatusCode Status { get; set; }

            public bool TimedOut { get; set;  }

            public Exception Exception { get; set; }
        }
    }
}

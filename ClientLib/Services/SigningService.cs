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
                        SignEachFile(location, f, strongName);

                    }
                });

            t.RunSynchronously();

            t.Wait();

        }



        private void CopyTo(IIoItem source, IIoItem destination)
        {
            SendMessage(new Message
                                {
                                    Contents = "Starting Copy '{0}' to '{1}'".format(source.FullName,
                                                                                     destination
                                                                                         .FullName),
                                    MessageType = MessageType.Info
                                });
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
        }

        



        private void SignEachFile(IAzureContainer container, SourceToDestinationMap<IIoItem> file, bool strongName)
        {


            var blob = IoItemFactory.Create(container.GetBlob(file.Source.Name.ToLower()));
            CopyTo(file.Source, blob);
            
            
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
                
            

            SendMessage(new Message
            {
                Contents = "Finished signing for {0}".format(file.Destination.FullName),
                MessageType = MessageType.Info
            });

            
            CopyTo(blob, file.Destination);
        }
    }
}

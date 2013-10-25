using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

using Outercurve.DTO.Response;
using Outercurve.DTO.Services.Azure;
using Outercurve.SigningApi.Signers;
using ServiceStack.Configuration;
using SigningServiceBase;

namespace Outercurve.SigningApi
{
    public class SigningJobCreator :  IDependency
    {
        private readonly IAzureClient _azureClient;
        private readonly LoggingService _log;
        private readonly ICertificateService _certs;


        public SigningJobCreator(IAzureClient azureClient, LoggingService log, ICertificateService certs)
        {
            _azureClient = azureClient;
            _log = log;
            _certs = certs;
        }

        public Job CreateJob(string container, string path, bool strongName)
        {
            return new Job
                {
                    Action = () => Sign(container, path, strongName),
                    FailedAction = () => FailedAction(container, path),
                    PrefixedAction = () => PrefixedAction(container, path),
                    PostFixedAction = () => PostfixAction(container, path)
                };
        }

        private void PrefixedAction(string container, string path)
        {
            var statusBlob = _azureClient.GetBlob(container, GetStatusPath(path));
            statusBlob.SaveTo(StatusCode.Running.ToString());
        }


        private void PostfixAction(string container, string path)
        {
            var statusBlob = _azureClient.GetBlob(container, GetStatusPath(path));
            statusBlob.SaveTo(StatusCode.Done.ToString());
        }

        private void FailedAction(string container, string path)
        {
            var statusBlob = _azureClient.GetBlob(container, GetStatusPath(path));
            statusBlob.SaveTo(StatusCode.Failed.ToString());
        }

        private void Sign(string container, string path, bool strongName)
        {
            try
            {
                var tempPath = _azureClient.CopyFileToTemp(container, path);
                var cert = _certs.Get();
                if (FileSensing.IsItAZipFile(tempPath))
                {
                    AttemptToSignOPC(tempPath, cert);
                    _log.Debug("OPC Signing is done");
                }
                else
                {
                    AttemptToSignAuthenticode(tempPath, strongName, cert);
                    _log.Debug("Authenticode is done");

                }
                _log.Debug(@"let's copy the file from {0} to {1}\{2}".format(tempPath, container, path));
                _azureClient.CopyFileToAzure(container, path, tempPath);
                _log.Debug(@"finished {0} to {1}\{2}".format(tempPath, container, path));


            }
            catch (Exception e)
            {
                _log.Fatal("error", e);
                throw;
            }
        }


        private void AttemptToSignAuthenticode(string path, bool strongName, X509Certificate2 certificate)
        {

            //_log.Debug(path);
            //_log.Debug(strongName);
            //_log.Debug(certificate);

            var authenticode = new AuthenticodeSigner(certificate, _log);
            AttemptToSign(() => authenticode.Sign(path, strongName));

        }

        private void AttemptToSignOPC(string path, X509Certificate2 certificate)
        {

            var opc = new OPCSigner(certificate, _log);

            AttemptToSign(() => opc.Sign(path, true));
        }

        private void AttemptToSign(Action signAction)
        {
            /* try
             {*/
            signAction();

            /*}
            catch (FileNotFoundException fnfe)
            {
                ThrowTerminatingError(new ErrorRecord(fnfe, "none", ErrorCategory.OpenError, null));
            }
            catch (PathTooLongException ptle)
            {
                ThrowTerminatingError(new ErrorRecord(ptle, "none", ErrorCategory.InvalidData, null));
            }
            catch (UnauthorizedAccessException uae)
            {
                ThrowTerminatingError(new ErrorRecord(uae, "none", ErrorCategory.PermissionDenied, null));
            }
            catch (Exception e)
            {
                ThrowTerminatingError(new ErrorRecord(e, "none", ErrorCategory.NotSpecified, null));
            }*/
            // return false;
        }

        public string GetStatusPath(string path)
        {
            return path + ".status";
        }

    }
}
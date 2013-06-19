using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ClrPlus.Core.Extensions;
using Microsoft.WindowsAzure.Storage.Blob;
using Outercurve.DTO.Request;
using Outercurve.DTO.Response;
using Outercurve.DTO.Services.Azure;
using Outercurve.SigningApi.Signers;
using ServiceStack.Configuration;
using ServiceStack.ServiceInterface;

namespace Outercurve.SigningApi
{
    public class ApiService : Service
    {
        private readonly IAzureService _azure;
        private readonly AzureClient _azureClient;
        private readonly FsService _fs;
        private readonly CertService _certs;
        private readonly AppSettings _settings;
        private readonly CustomBasicAuthProvider _authProvider;
        private readonly List<String> Errors = new List<string>();
        private readonly LoggingService _log;
        private readonly JobScheduler _jobScheduler;
        private readonly SigningJobCreator _jobCreator;
        public const int HOURS_FILE_SHOULD_BE_ACCESSIBLE = 12;

        public ApiService(AzureClient azureClient, FsService fs, CertService certs, AppSettings settings, CustomBasicAuthProvider authProvider, 
            LoggingService log, JobScheduler jobScheduler, SigningJobCreator jobCreator)
        {
            _azure = azureClient.GetRoot();
            _azureClient = azureClient;
            _fs = fs;
            _certs = certs;
            _settings = settings;
            _authProvider = authProvider;
            _log = log;
            _jobScheduler = jobScheduler;
            _jobCreator = jobCreator;
        }

        public GetUploadLocationResponse Post(GetUploadLocationRequest request)
        {
            _log.StartLog(request);
            try
            {

            
                var contName = "deletable" + Guid.NewGuid().ToString("D").ToLowerInvariant();

                var container = _azure.CreateContainerIfDoesNotExist(contName);

                var blobPolicy = new SharedAccessBlobPolicy {
                                                                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List,
                                                                SharedAccessExpiryTime = DateTimeOffset.Now.AddHours(HOURS_FILE_SHOULD_BE_ACCESSIBLE)
                                                            
                                                            };

                var permissions = new BlobContainerPermissions();
                permissions.SharedAccessPolicies.Add("mypolicy", blobPolicy);
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                container.SetPermissions(permissions);
                var sharedAccessSignature = container.GetSharedAccessSignature("mypolicy");

                return new GetUploadLocationResponse {Name= container.Name, Location = container.Uri.ToString(), Sas = sharedAccessSignature, Account = _azure.Account};
            }
            catch (Exception e)
            {
                _log.Fatal("error", e);
                Errors.Add(e.Message + " " + e.StackTrace);

                return new GetUploadLocationResponse {Errors = Errors};
            }
        }



        public SetCodeSignatureResponse Post(SetCodeSignatureRequest request)
        {
            _log.StartLog(request);
           
            
            //Errors.Add(" cert is " + cert.SerialNumber);
            try
            {
                _azureClient.GetBlob(request.Container, _jobCreator.GetStatusPath(request.Path)).SaveTo(StatusCode.WaitingToRun.ToString());
                _jobScheduler.Add(_jobCreator.CreateJob(request.Container, request.Path, request.StrongName));
                return new SetCodeSignatureResponse();
            }
            catch (Exception e)
            {
                _log.Fatal("error", e);
                Errors.Add(e.Message + " " + e.StackTrace);
                return new SetCodeSignatureResponse { Errors = Errors };
            }
        }

        public BaseResponse Post(SetRolesRequest request)
        {
            _log.StartLog(request);
            try
            {
                _authProvider.SetRoles(request.UserName, request.Roles.ToArray());
                return new BaseResponse();
            }
            catch (Exception e)
            {
                _log.Fatal("error", e);
                Errors.Add(e.Message + " " + e.StackTrace);
                return new BaseResponse { Errors = Errors };
            }
                

        }


        public GetRolesResponse Post(GetRolesAsAdminRequest request)
        {
            _log.StartLog(request);
            try
            {
                var roles = _authProvider.GetRoles(request.UserName);
                return new GetRolesResponse {Roles = roles.ToList()};
            }
            catch (Exception e)
            {
                _log.Fatal("error", e );
                Errors.Add(e.Message + " " + e.StackTrace);
                return new GetRolesResponse {Errors = Errors};
            }
        }

        public GetStatusResponse Post(GetStatus request)
        {
            _log.StartLog(request);
            try
            {
                var status = _azureClient.GetBlob(request.Container, _jobCreator.GetStatusPath(request.Path)).GetText();
                if (String.IsNullOrWhiteSpace(status))
                {
                    throw new Exception(@"Status couldn't be retrieved for {0}\{1}".format(request.Container,
                                                                                           request.Path));
                }

                var result = (StatusCode) Enum.Parse(typeof (StatusCode), status);
                return new GetStatusResponse {Status = result};
            }
             
            catch (Exception e)
            {
                _log.Fatal("error", e);
                Errors.Add(e.Message + " " + e.StackTrace);
                return new GetStatusResponse { Errors = Errors };
            }
        }

        public GetRolesResponse Post(GetRolesRequest request)
        {
            _log.StartLog(request);
            try
            {

                var roles = _authProvider.GetRoles(this.GetSession().UserAuthName);
                return new GetRolesResponse {Roles = roles.ToList()};
            }
            catch (Exception e)
            {
                _log.Fatal("error", e);
                Errors.Add(e.Message + " " + e.StackTrace);
                return new GetRolesResponse {Errors = Errors};
            }
        }

        public BaseResponse Post(UnsetRolesRequest request)
        {
            _log.StartLog(request);
            try
            {
                _authProvider.UnsetRoles(request.UserName, request.Roles.ToArray());
                return new BaseResponse();
            }
            catch (Exception e)
            {
                _log.Fatal("error", e);
                Errors.Add(e.Message + " " + e.StackTrace);
                return new SetCodeSignatureResponse { Errors = Errors };
            }
                
        }

        public CreateUserResponse Post(ResetPasswordAsAdminRequest request)
        {
            _log.StartLog(request);
            try
            {
                var pass = _authProvider.ResetPasswordAsAdmin(request.UserName);
                return new CreateUserResponse {Password = pass};
            }
            catch (Exception e)
            {
                _log.Fatal("error", e);
                Errors.Add(e.Message + " " + e.StackTrace);
                return new CreateUserResponse { Errors = Errors };
            }
        }

        public BaseResponse Post(RemoveUserRequest request)
        {
            _log.StartLog(request);
            try
            {
                _authProvider.RemoveUser(request.UserName);
                return new BaseResponse();
            }
            catch (Exception e)
            {
                _log.Fatal("error", e);
                Errors.Add(e.Message + " " + e.StackTrace);
                return new BaseResponse { Errors = Errors };
            }
        }

        public BaseResponse Post(SetPasswordRequest request)
        {
            _log.StartLog(request);
            try
            {
                _authProvider.SetPassword(this.GetSession().UserAuthName, request.NewPassword);
                return new BaseResponse();
            }
            catch (Exception e)
            {
                _log.Fatal("error", e);
                Errors.Add(e.Message + " " + e.StackTrace);
                return new CreateUserResponse { Errors = Errors };
            }
        }

        public CreateUserResponse Post(CreateUserRequest request)
        {
            _log.StartLog(request);
            try
            {
                if (String.IsNullOrWhiteSpace(request.Password))
                {
                    var password = _authProvider.CreateUser(request.Username);
                    if (password != null)
                    {
                        return new CreateUserResponse {Password = password};
                    }
                        throw new Exception("User could not be created. May already be registered?");
                    

                }
                else
                {
                   if (_authProvider.CreateUserWithPassword(request.Username, request.Password))
                   {
                       return new CreateUserResponse();
                   }

                   throw new Exception("User could not be created. May already be registered?");
                    
                }
            }
            catch (Exception e)
            {
                _log.Fatal("error", e);
                Errors.Add(e.Message + " " + e.StackTrace);
                return new CreateUserResponse {Errors = Errors};
            }
        }

        public BaseResponse Post(InitializeRequest request)
        {
            _log.StartLog(request);
            try
            {
                _authProvider.Initialize(request.UserName, request.Password);
                return new BaseResponse();
            }
            catch (Exception e)
            {
                _log.Fatal("error", e);
                Errors.Add(e.Message + " " + e.StackTrace);
                return new BaseResponse { Errors = Errors };
            }
        }



        
    }
}
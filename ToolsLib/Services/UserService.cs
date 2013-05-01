using System;
using System.Collections.Generic;
using System.Linq;
using Outercurve.DTOs.Request;
using Outercurve.ToolsLib.Messages;

namespace Outercurve.ToolsLib.Services
{
    public class UserService : Service
    {
        public UserService(string username, string password, string serviceUrl, Action<Message> messageHandler = null,
                              Action<ProgressMessage> progressHandler = null)
            : base(username, password, serviceUrl, messageHandler, progressHandler)
        {

        }

        /// <summary>
        /// for uncredentialied requests
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="messageHandler"></param>
        /// <param name="progressHandler"></param>
        public UserService(string serviceUrl, Action<Message> messageHandler = null,
                           Action<ProgressMessage> progressHandler = null)
            : base(serviceUrl, messageHandler, progressHandler)
        {
            
        }

        public string CreateUser(string user)
        {
                var r = Client.Post(new CreateUserRequest {Username = user});
                ThrowErrors(r.Errors);
                return r.Password;
        }

        public void CreateUserWithPassword(string user, string password)
        {
            var r = Client.Post(new CreateUserRequest {Password = password, Username = user});
            ThrowErrors(r.Errors);
            
        }

        public IEnumerable<string> GetRoles(string user)
        {
            var r = Client.Post(new GetRolesRequest {UserName = user});
            ThrowErrors(r.Errors);
            return r.Roles;
        }

        public void SetRoles(string user, IEnumerable<string> roles)
        {
            var r = Client.Post(new SetRolesRequest { UserName = user, Roles =  roles.ToList()});
            ThrowErrors(r.Errors);
            
        }

        public void UnsetRoles(string user, IEnumerable<string> roles)
        {
            var r = Client.Post(new UnsetRolesRequest { UserName = user, Roles = roles.ToList() });
            ThrowErrors(r.Errors);
        }

        public void SetPassword(string newPassword)
        {
            var r = Client.Post(new SetPasswordRequest {NewPassword = newPassword});
            ThrowErrors(r.Errors);
        }

        public string ResetPasswordAsAdmin(string user)
        {
            var r = Client.Post(new ResetPasswordAsAdminRequest {UserName = user});
            ThrowErrors(r.Errors);
            return r.Password;
        }

        public void Initialize(string userName, string password)
        {
            var r = Client.Post(new InitializeRequest { UserName = userName, Password =  password});
            ThrowErrors(r.Errors);
        }
    }
}

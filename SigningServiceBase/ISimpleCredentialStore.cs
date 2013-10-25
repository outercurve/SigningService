using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;

namespace SigningServiceBase
{
    public interface ISimpleCredentialStore : IAuthProvider, IDependency
    {
        void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, Dictionary<string, string> authInfo);

        bool TryAuthenticate(IServiceBase authService, string username, string password);
        string CreateUser(string userName);
        void UnsetRoles(string userName, params string[] roles);
        IEnumerable<string> GetRoles(string userName);
        bool SetRoles(string userName, params string[] roles);

        bool RemoveUser(string userName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool CreateUserWithPassword(string userName, string password);

        string ResetPasswordAsAdmin(string userName);
        bool SetPassword(string userName, string newPassword);

        void Initialize(string userName, string password);

    }
}

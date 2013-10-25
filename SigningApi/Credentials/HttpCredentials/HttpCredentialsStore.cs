using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using SigningServiceBase;

namespace Outercurve.HttpCredentials
{
    public class HttpCredentialsStore : BasicAuthProvider, ISimpleCredentialStore
    {
        private readonly string _baseUrl;

        public HttpCredentialsStore(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, Dictionary<string, string> authInfo)
        {
            session.IsAuthenticated = true;
            authService.SaveSession(session);
        }

        public bool TryAuthenticate(IServiceBase authService, string username, string password)
        {

            return TryAuthenticate(username, password).Result;
        }

        private async Task<bool> TryAuthenticate(string username, string password)
        {
            try
            {
                var client = new HttpClient();

                HttpContent content = new FormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password)
                });

                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                {
                    CharSet = "UTF-8"
                };


                var t = await client.PostAsync(_baseUrl, content);
                if (!t.IsSuccessStatusCode)
                    return false;

                bool result = false;

                return t.TryGetContentValue(out result) && result;


            }
            catch (Exception)
            {

                return false;
            }
        }

        public string CreateUser(string userName)
        {
            throw new NotImplementedException();
        }

        public void UnsetRoles(string userName, params string[] roles)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetRoles(string userName)
        {
            throw new NotImplementedException();
        }

        public bool SetRoles(string userName, params string[] roles)
        {
            throw new NotImplementedException();
        }

        public bool CreateUserWithPassword(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public string ResetPasswordAsAdmin(string userName)
        {
            throw new NotImplementedException();
        }

        public bool SetPassword(string userName, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void Initialize(string userName, string password)
        {
            //no-op
        }
    }
}

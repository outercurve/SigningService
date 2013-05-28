using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ClrPlus.Core.Collections;
using ClrPlus.Scripting.Languages.PropertySheet;
using ServiceStack.Configuration;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;

namespace Outercurve.SigningApi
{
    
        public class CustomBasicAuthProvider : BasicAuthProvider
        {

            public const int DEFAULT_PASS_LENGTH = 16;

            private readonly object _lock = new object();
            private readonly AppHost _hostBase;
            private readonly AppSettings _settings;
            private readonly LoggingService _log;
            public CustomBasicAuthProvider(AppHost hostBase, AppSettings settings, LoggingService log)
            {
                _hostBase = hostBase;
                _settings = settings;
                _log = log;
            }

            public override bool TryAuthenticate(IServiceBase authService, string userName, string password)
            {
                
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    _log.StartAuthenticate(userName, password, false);
                    return false;
                }
                lock (_lock)
                {
                    var user = GetUserRule(userName);

                    if (user == null)
                    {
                        return false;
                    }

                    var storedPassword = user["password"].Value;

                    if (storedPassword.Length == 32)
                    {

                        var pwd = HashPassword(password);
                        if (pwd == storedPassword)
                        {
                            _log.StartAuthenticate(userName, password, true);
                            return true;
                        }
                    }

                 /*   if (storedPassword == password)
                    {
                        // matched against password unsalted.
                        // user should change password asap.
                        return true;
                    }*/
                    _log.StartAuthenticate(userName, password, false);
                    return false;
                }

            }

            public override void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, Dictionary<string, string> authInfo)
            {
                lock (_lock)
                {
                    var user = GetUserRule(session.UserAuthName);

                    if (user == null)
                    {
                        return;
                    }

                    session.IsAuthenticated = true;
                    //Fill the IAuthSession with data which you want to retrieve in the app eg:
                    session.FirstName = user.HasProperty("firstname") ? user["firstname"].Value : "";
                    session.LastName = user.HasProperty("lastname") ? user["lastname"].Value : "";

                    session.Roles = new XList<string>();

                    // check to see if user has an unsalted password
                    var storedPassword = user["password"].Value;
                  /*  if (storedPassword.Length != 32)
                    {
                        session.Roles.Add("password_must_be_changed");
                    }*/

                    if (user.HasProperty("roles"))
                    {
                        session.Roles.AddRange(user["roles"].Values);
                    }

                    //Important: You need to save the session!
                    authService.SaveSession(session, SessionExpiry);
                }
            }
           

           

            private string HashPassword(string password)
            {
                using (var hasher = MD5.Create())
                {
                    return
                        hasher.ComputeHash(Encoding.Unicode.GetBytes(AppHost.ServiceName + password))
                              .Aggregate(String.Empty, (current, b) => current + b.ToString("x2").ToUpper());
                }
            }

            public string CreateUser(string userName)
            {



                var password = RandomPasswordGenerator.GeneratePassword(DEFAULT_PASS_LENGTH);
                if (CreateUserWithPassword(userName, password))
                {
                    return password;
                }
                return null;
            }

            public void UnsetRoles(string userName, params string[] roles)
            {
                lock (_lock)
                {
                    var path = UserPropertySheetPath;

                    if (path != null)
                    {
                        var propertySheet = UserPropertySheet;

                        var user =
                            propertySheet.Rules.FirstOrDefault(rule => rule.Name == "user" && rule.Parameter == userName);

                        RemoveRolesFromUser(user, roles);
                    }
                }
            }

            public IEnumerable<string> GetRoles(string userName)
            {
                lock (_lock)
                {
                    var path = UserPropertySheetPath;

                    if (path != null)
                    {
                        var propertySheet = UserPropertySheet;

                        var user =
                            propertySheet.Rules.FirstOrDefault(rule => rule.Name == "user" && rule.Parameter == userName);


                        if (user == null)
                        {

                            //user doesn't exists!
                            return null;
                        }

                        return GetRolesPV(user).Values;
                    }

                }

                return null;
            }

            public bool SetRoles(string userName, params string[] roles)
            {
                lock (_lock)
                {
                    var path = UserPropertySheetPath;

                    if (path != null)
                    {
                        var propertySheet = UserPropertySheet;

                        var user =
                            propertySheet.Rules.FirstOrDefault(rule => rule.Name == "user" && rule.Parameter == userName);


                        if (user == null)
                        {

                            //user doesn't exists!
                            return false;
                        }
                        

                        SetRolesToUser(user, roles);

                        propertySheet.Save(path);
                        return true;
                    }

                }
                return false;
            }


            public void Initialize(string userName, string password)
            {
                lock (_lock)
                {
                    var path = UserPropertySheetPath;

                    if (path != null)
                    {
                        var propertySheet = UserPropertySheet;

                        var anyUsers =
                            propertySheet.Rules.Any(rule => rule.Name == "user");
                        if (!anyUsers)
                        {
                           if (CreateUserWithPassword(userName, password))
                           {
                               SetRoles(userName, "admins");
                               return;
                           }
                           else
                           {
                               throw new Exception("Can't set user for some reason");
                           }
                            

                        }
                        throw new Exception("System is already initialized");
                    }
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="userName"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public bool CreateUserWithPassword(string userName, string password)
            {
                lock (_lock)
                {
                    var path = UserPropertySheetPath;

                    if (path != null)
                    {
                        var propertySheet = UserPropertySheet;
                        var user =
                            propertySheet.Rules.FirstOrDefault(rule => rule.Name == "user" && rule.Parameter == userName);

                        
                        if (user != null)
                        {

                            //user already exists!
                            return false;
                        }


                        user = propertySheet.AddRule("user", userName);
                        var propRule = user.GetPropertyRule("password");
                        var pv  = propRule.GetPropertyValue(string.Empty);
                        pv.Add(HashPassword(password));
                  
                        propertySheet.Save(path);

                        
                        return true;
                        
                    }
                }
                return false;

            }

            public string ResetPasswordAsAdmin(string userName)
            {
                lock (_lock)
                {
                    var path = UserPropertySheetPath;

                    if (path != null)
                    {
                        var propertySheet = UserPropertySheet;
                       
                        var user =
                           propertySheet.Rules.FirstOrDefault(rule => rule.Name == "user" && rule.Parameter == userName);


                        if (user == null)
                        {

                            //user doesn't exists!
                            return null;
                        }

                        var password = RandomPasswordGenerator.GeneratePassword(DEFAULT_PASS_LENGTH);
                        var propRule = user.GetPropertyRule("password");
                        var pv = propRule.GetPropertyValue(string.Empty);
                     
                     
                        pv.Add(HashPassword(password));
                        propertySheet.Save(path);


                        return password;
                        

                    }
                }

                return null;
            }

            public bool SetPassword(string userName, string newPassword)
            {
                lock (_lock)
                {
                    var path = UserPropertySheetPath;

                    if (path != null)
                    {
                        var propertySheet = UserPropertySheet;

                        var user =
                            propertySheet.Rules.FirstOrDefault(rule => rule.Name == "user" && rule.Parameter == userName);


                        if (user == null)
                        {

                            //user doesn't exists!
                            return false;
                        }

                        
                        var propRule = user.GetPropertyRule("password");
                        var pv = propRule.GetPropertyValue(string.Empty);

                       
                       pv.Clear();

                        pv.Add(HashPassword(newPassword));
                        propertySheet.Save(path);

                        return true;

                    }
                }

                return false;
            }



            internal string UserPropertySheetPath
            {
                get
                {
                    
                    var p = GetUserPathAppSettings();
                    if (p != null)
                    {
                            
                        p = HttpContext.Current.Server.MapPath(p);
                        if (File.Exists(p))
                        {
                            return p;
                        }
                    }
                    
                    return null;
                }
            }

            internal PropertySheet UserPropertySheet
            {
                get
                {
                    var path = UserPropertySheetPath;
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        return PropertySheet.Load(path);
                    }
                    // otherwise you get an empty propertySheet.
                    return new PropertySheet();
                }
            }


            private void SetRolesToUser(Rule user, IEnumerable<string> roles)
            {

                
                var rolesVal = GetRolesPV(user);
             

                SetPropertyValues(rolesVal, roles.Select(s => s.ToLowerInvariant()).ToArray());
                
            }

            private PropertyValue GetRolesPV(Rule user)
            {
                var rolesRule = user.GetPropertyRule("roles");
                return rolesRule.GetPropertyValue(string.Empty);
            }

            private void RemoveRolesFromUser(Rule user, IEnumerable<string> roles)
            {
                var rolesRule = user.GetPropertyRule("roles");
                var rolesVal = rolesRule.GetPropertyValue(string.Empty);
               

                var origRoles = rolesVal.Values.ToList();
                SetPropertyValues( rolesVal, origRoles.Except(roles.Select(s => s.ToLowerInvariant())).ToArray());
            }

            private void SetPropertyValues(PropertyValue pv, IEnumerable<string> values)
            {
               pv.Clear();
                
                foreach (var v in values)
                {
                    pv.Add(v);
                }
            }

            internal IEnumerable<Rule> UserRules
            {
                get
                {
                    return UserPropertySheet.Rules.Where(rule => rule.Name == "user");
                }
            }

            private Rule GetUserRule(string name)
            {
                return UserRules.FirstOrDefault(rule => rule.Parameter == name);
            }

            private string GetUserPathAppSettings()
            {
                var a = _settings.GetString("Users");
                return a;
            }

            internal bool RemoveUser(string userName)
            {
                lock (_lock)
                {
                    var path = UserPropertySheetPath;

                    if (path != null)
                    {
                        var propertySheet = UserPropertySheet;

                        var user =
                            propertySheet.Rules.FirstOrDefault(rule => rule.Name == "user" && rule.Parameter == userName);


                        if (user == null)
                        {

                            //user doesn't exists!
                            return false;
                        }

                        propertySheet.RemoveRule(user);
                        
                        propertySheet.Save(path);
                        return true;

                    }
                }

                return false;
            }
        }
    
}
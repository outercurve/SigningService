using System;
using System.Security.Cryptography;
using System.Text;

namespace Outercurve.SigningApi
{
    public class RandomPasswordGenerator
    {
        private static readonly char[] _alphaNumChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <from>https://github.com/mono/mono/blob/master/mcs/class/System.Web/System.Web.Security/Membership.cs</from>
        /// <license>MIT</license>
        public static string GeneratePassword(int length)
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            var pass_bytes = new byte[length];
            int i;
           

            rng.GetBytes(pass_bytes);

            for (i = 0; i < length; i++)
            {
                /* convert the random bytes to ascii values 33-126 */
                pass_bytes[i] = (byte)(pass_bytes[i] % _alphaNumChars.Length);
                //48-57 65-90 97-122

                pass_bytes[i] = Convert.ToByte(_alphaNumChars[pass_bytes[i]]);
            }

            return Encoding.ASCII.GetString(pass_bytes);
        }
    }
}
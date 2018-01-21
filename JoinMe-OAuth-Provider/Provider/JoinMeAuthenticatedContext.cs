//  Copyright 2017 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Owin.Security.Providers.JoinMe
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class JoinMeAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="JoinMeAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized user</param>
        /// <param name="accessToken">JoinMe access token</param>
        public JoinMeAuthenticatedContext(
            IOwinContext context, string accessToken, string expires, string refreshToken, JObject userJson) 
            : base(context)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;

            int expiresValue;
            if (Int32.TryParse(expires, NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresValue)) 
            {
                ExpiresIn = TimeSpan.FromSeconds(expiresValue);
            }

            // per https://developer.join.me/docs/read/users
            Email = userJson["email"]?.Value<string>();            
            FullName = userJson["fullName"]?.Value<string>();
            AccountType = userJson["subscriptionType"]?.Value<string>();

            // UserId should come from the OAuth provider, 
            // but join.me doesn't provide one, so we have to generate a synthetic UserId.
            // Warning: if user changes email address associated with their join.me account 
            // (which they can) this OAuth provider will not recognize them as the same user.
            UserId = ComputeMd5Hash(Email);
        }

        /// <summary>
        /// Gets the JoinMe OAuth access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the scope for this JoinMe OAuth access token
        /// </summary>
        public string[] Scope { get; private set; }

        /// <summary>
        /// Gets the JoinMe access token expiration time
        /// </summary>
        public TimeSpan? ExpiresIn { get; private set; }

        /// <summary>
        /// Gets the JoinMe OAuth refresh token
        /// </summary>
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Gets the JoinMe user ID
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the email address
        /// </summary>
        public string Email { get; private set; }
        
        /// <summary>
        /// Gets the user's last name
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Gets the email address
        /// </summary>
        public string AccountType { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private static string ComputeMd5Hash(string input) 
        {
            if (String.IsNullOrEmpty(input)) return null;

            byte[] bytes;
            using (var md5Hash = System.Security.Cryptography.MD5.Create()) 
            {
                bytes = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            }

            return String.Concat(bytes.Select(x => x.ToString("x2")));
        }
    }
}

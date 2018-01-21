//  Copyright 2017 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;

namespace Owin.Security.Providers.JoinMe
{
    public static class JoinMeAuthenticationExtensions
    {
        public static IAppBuilder UseJoinMeAuthentication(this IAppBuilder app, JoinMeAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (options == null)
                throw new ArgumentNullException("options");

            app.Use(typeof(JoinMeAuthenticationMiddleware), app, options);

            return app;
        }

        public static IAppBuilder UseJoinMeAuthentication(this IAppBuilder app, string clientId, string clientSecret)
        {
            return app.UseJoinMeAuthentication(new JoinMeAuthenticationOptions
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            });
        }
    }
}
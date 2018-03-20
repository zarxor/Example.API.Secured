    // -------------------------------------------------------------------------------------------------
    // Copyright (c) Johan Boström. All rights reserved.
    // Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
    // -------------------------------------------------------------------------------------------------

    namespace API.BasicAuth.Attributes
    {
        using System;
        using System.Text;
        using Microsoft.AspNetCore.Mvc;
        using Microsoft.AspNetCore.Mvc.Filters;

        public class BasicAuthorizeFilter : IAuthorizationFilter
        {
            private readonly string realm;

            public BasicAuthorizeFilter(string realm = null)
            {
                this.realm = realm;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                string authHeader = context.HttpContext.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    // Get the encoded username and password
                    var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();

                    // Decode from Base64 to string
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    // Split username and password
                    var username = decodedUsernamePassword.Split(':', 2)[0];
                    var password = decodedUsernamePassword.Split(':', 2)[1];

                    // Check if login is correct
                    if (IsAuthorized(username, password))
                    {
                        return;
                    }
                }

                // Return authentication type (causes browser to show login dialog)
                context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic";

                // Add realm if it is not null
                if (!string.IsNullOrWhiteSpace(realm))
                {
                    context.HttpContext.Response.Headers["WWW-Authenticate"] += $" realm=\"{realm}\"";
                }

                // Return unauthorized
                context.Result = new UnauthorizedResult();
            }

            // Make your own implementation of this
            public bool IsAuthorized(string username, string password)
            {
                // Check that username and password are correct
                return username.Equals("User1", StringComparison.InvariantCultureIgnoreCase)
                       && password.Equals("SecretPassword!");
            }
        }
    }
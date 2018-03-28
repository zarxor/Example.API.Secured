// -------------------------------------------------------------------------------------------------
// Copyright (c) Johan Boström. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// -------------------------------------------------------------------------------------------------

namespace API.BasicAuth.Handler
{
    using System;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected new BasicAuthenticationEvents Events
        {
            get => (BasicAuthenticationEvents) base.Events;
            set => base.Events = value;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authHeader = Request.Headers["Authorization"];

            // No auth header
            if (authHeader == null
                || AuthenticationHeaderValue.TryParse(authHeader, out var authenticationHeaderValue)
                || !authenticationHeaderValue.Scheme.Equals("Basic", StringComparison.InvariantCultureIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }

            // Decode from Base64 to string
            var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationHeaderValue.Parameter));

            // Split username and password
            var splittedUsernamePassword = decodedUsernamePassword.Split(':');
            if (splittedUsernamePassword.Length != 2)
            {
                return AuthenticateResult.Fail("Error parsing username or password");
            }

            var username = splittedUsernamePassword[0];
            var password = splittedUsernamePassword[1];

            return await Events.OnValidate(username, password, Context);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            return base.HandleForbiddenAsync(properties);
        }
    }

    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; }
        public new BasicAuthenticationEvents Events { get; set; }
    }

    public class BasicAuthenticationEvents
    {
        public OnValidateBasicAuthentication OnValidate { get; set; }
    }

    public delegate Task<AuthenticateResult> OnValidateBasicAuthentication(string username, string password, HttpContext context);
}
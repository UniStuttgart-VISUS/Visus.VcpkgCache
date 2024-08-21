// <copyright file="TokenAuthenticationHandler.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for more information.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Visus.VcpkgCache.Properties;


namespace Visus.VcpkgCache {

    /// <summary>
    /// A custom authentication handler that checks for a token in one of
    /// the configured HTTP headers.
    /// </summary>
    internal sealed class TokenAuthenticationHandler(
            IOptionsMonitor<TokenAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder urlEncoder)
        : AuthenticationHandler<TokenAuthenticationOptions>(
            options,
            loggerFactory,
            urlEncoder) {

        #region Protected methods
        protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
            if (string.IsNullOrWhiteSpace(this.Options.Token)) {
                var result = AuthenticateResult.Fail(
                    Resources.ErrorMissingToken);
                return Task.FromResult(result);
            }

            foreach (var n in this.Options.HeaderNames) {
                if (this.Request.Headers.ContainsKey(n)) {
                    var header  = this.Request.Headers[n];

                    foreach (var v in header) {
                        var parts = v?.Split();

                        if ((parts != null)
                                && (parts.Length == 2)
                                && (parts[0] == "Token")) {
                            if (parts[1] == this.Options.Token) {
                                var result = AuthenticateResult.Success(
                                    this.IssueTicket(parts[1]));
                                return Task.FromResult(result);
                            } else {
                                var result = AuthenticateResult.Fail(
                                    Resources.ErrorInvalidToken);
                                return Task.FromResult(result);
                            }
                        }
                    }
                }
            }

            {
                var msg = Resources.ErrorMissingHeader;
                msg = string.Format(msg, this.Request.Headers.First());
                var result = AuthenticateResult.Fail(msg);
                return Task.FromResult(result);
            }
        }
        #endregion

        #region Private methods
        private AuthenticationTicket IssueTicket(string token) {
            Debug.Assert(token != null);
            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.Authentication, token)],
                this.Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            return new AuthenticationTicket(principal, this.Scheme.Name);
        }
        #endregion
    }
}

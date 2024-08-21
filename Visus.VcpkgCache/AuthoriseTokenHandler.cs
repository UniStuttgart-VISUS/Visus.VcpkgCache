// <copyright file="AuthoriseTokenHandler.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for more information.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;


namespace Visus.VcpkgCache {

    /// <summary>
    /// A custom authorisation handler for token-based access control.
    /// </summary>
    internal sealed class AuthoriseTokenHandler
            : AuthorizationHandler<AuthoriseTokenAttribute> {

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="contextAccessor"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AuthoriseTokenHandler(IOptions<Settings> settings,
                IHttpContextAccessor contextAccessor,
                ILogger<AuthoriseTokenHandler> logger) {
            this._contextAccessor = contextAccessor
                ?? throw new ArgumentNullException(nameof(contextAccessor));
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this._settings = settings?.Value
                ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <inheritdoc />
        protected override Task HandleRequirementAsync(
                AuthorizationHandlerContext context,
                AuthoriseTokenAttribute requirement) {
            var httpContext = this._contextAccessor.HttpContext;

            if (httpContext?.Request?.Headers != null) {
                foreach (var h in this._settings.AuthorisationHeaders) {
                    var header = httpContext.Request.Headers[h];

                    foreach (var v in header) {
                        var parts = v?.Split();

                        if ((parts != null)
                                && (parts.Length == 2)
                                && (parts[0] == "Token")
                                && (parts[1] == this._settings.Token)) {
                            context.Succeed(requirement);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        #region Private fields
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger _logger;
        private readonly Settings _settings;
        #endregion
    }
}

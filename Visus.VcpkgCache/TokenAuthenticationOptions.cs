// <copyright file="TokenAuthenticationOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for more information.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.AspNetCore.Authentication;


namespace Visus.VcpkgCache {

    /// <summary>
    /// Configures the <see cref="TokenAuthenticationHandler"/>.
    /// </summary>
    internal sealed class TokenAuthenticationOptions
            : AuthenticationSchemeOptions {

        /// <summary>
        /// The name of the authentication scheme.
        /// </summary>
        public const string DefaultScheme = "TokenScheme";

        /// <summary>
        /// The name of the configuration section to be mapped to this object.
        /// </summary>
        public const string Section = "TokenAuthentication";

        /// <summary>
        /// Gets or sets the names of the headers that are checked.
        /// </summary>
        public string[] HeaderNames { get; set; } = ["Authorization"];

        /// <summary>
        /// Gets or sets the token we expect.
        /// </summary>
        public string Token { get; set; } = null!;
    }
}

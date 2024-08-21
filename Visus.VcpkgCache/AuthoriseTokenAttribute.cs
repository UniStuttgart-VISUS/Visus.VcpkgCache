// <copyright file="AuthoriseTokenAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for more information.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;


namespace Visus.VcpkgCache {

    /// <summary>
    /// An attribute that marks a request to require the authorisation token.
    /// </summary>
    internal sealed class AuthoriseTokenAttribute : AuthorizeAttribute,
            IAuthorizationRequirement, IAuthorizationRequirementData {

        /// <inheritdoc />
        public IEnumerable<IAuthorizationRequirement> GetRequirements() {
            yield return this;
        }
    }
}

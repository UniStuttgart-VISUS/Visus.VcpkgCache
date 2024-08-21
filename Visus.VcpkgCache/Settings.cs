// <copyright file="Settings.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for more information.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.VcpkgCache {

    /// <summary>
    /// Holds the application configuration.
    /// </summary>
    public sealed class Settings {

        /// <summary>
        /// Gets the name of the section in the application settings that maps
        /// to this object.
        /// </summary>
        public const string Section = "CacheSettings";

        /// <summary>
        /// Gets or sets the headers to be checked for the authorisation token.
        /// </summary>
        public string[] AuthorisationHeaders { get; set; } = ["Authorization"];

        /// <summary>
        /// Gets or sets the path where the packages are stored.
        /// </summary>
        public string Path { get; set; } = null!;

        /// <summary>
        /// Gets or sets the API token that is required for storing new packages.
        /// </summary>
        public string Token { get; set; } = null!;
    }
}

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
        /// Gets or sets the path where the packages are stored.
        /// </summary>
        public string Path { get; set; } = null!;

    }
}

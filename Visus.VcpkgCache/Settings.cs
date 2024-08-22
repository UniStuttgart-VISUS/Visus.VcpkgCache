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
        /// Gets or sets an extension that is added to cached files.
        /// </summary>
        public string? EnforceExtension {
            get => this._enforceExtension;
            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    value = null;
                }

                if ((value != null) && (value[0] != '.')) {
                    value = $".{value}";
                }

                this._enforceExtension = value;
            }
        }

        /// <summary>
        /// Gets or sets the path where the packages are stored.
        /// </summary>
        public string Path { get; set; } = null!;

        /// <summary>
        /// Gets or sets the authentication token we expect for PUT requests.
        /// </summary>
        public string Token { get; set; } = null!;

        #region Private fields
        private string? _enforceExtension;
        #endregion
    }
}

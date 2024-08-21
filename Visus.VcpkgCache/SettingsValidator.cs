// <copyright file="SettingsValidator.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for more information.
// </copyright>
// <author>Christoph Müller</author>

using FluentValidation;
using System.IO;
using Visus.VcpkgCache.Properties;


namespace Visus.VcpkgCache {

    /// <summary>
    /// Holds the application configuration.
    /// </summary>
    internal sealed class SettingsValidator : AbstractValidator<Settings> {

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        public SettingsValidator() {
            this.RuleFor(c => c.AuthorisationHeaders)
                .NotEmpty();
            this.RuleFor(c => c.Path)
                .NotEmpty()
                .Must(Directory.Exists)
                .WithMessage(Resources.ErrorInvalidCachePath);
            this.RuleFor(c => c.Token)
                .NotEmpty();
        }
    }
}

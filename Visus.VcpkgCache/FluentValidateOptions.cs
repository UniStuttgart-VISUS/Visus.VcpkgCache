// <copyright file="FluentValidateOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for more information.
// </copyright>
// <author>Christoph Müller</author>

using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Linq;


namespace Visus.VcpkgCache {

    /// <summary>
    /// Wraps a fluent <see cref="TValidator"/> to implement
    /// <see cref="IValidateOptions{TOptions}"/>.
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TValidator"></typeparam>
    internal sealed class FluentValidateOptions<TOptions, TValidator>
            : IValidateOptions<TOptions>
            where TOptions : class
            where TValidator : AbstractValidator<TOptions>, new() {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="validator"></param>
        public FluentValidateOptions(TValidator? validator = null) {
            this._validator = validator ?? new();
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string? name,
                TOptions options) {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            var result = this._validator.Validate(options);

            return result.IsValid
                ? ValidateOptionsResult.Success
                : ValidateOptionsResult.Fail(result.Errors.Select(e => e.ErrorMessage));
        }
        #endregion

        #region Private fields
        private readonly TValidator _validator;
        #endregion
    }
}

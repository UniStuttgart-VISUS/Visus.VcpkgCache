// <copyright file="PackageController.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for more information.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Visus.VcpkgCache.Properties;
using IoFile = System.IO.File;


namespace Visus.VcpkgCache.Controllers {

    /// <summary>
    /// The controller for the package cache.
    /// </summary>
    [ApiController]
    [Route("")]
    public class PackageController : ControllerBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PackageController(IOptions<Settings> settings,
                ILogger<PackageController> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this._settings = settings?.Value
                ?? throw new ArgumentNullException(nameof(settings));
        }
        #endregion

        /// <summary>
        /// Gets the specified package.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("{path}")]
        public IActionResult Get(string path) {
            if (path.ContainsInvalidFileNameChars()) {
                return this.BadRequest(Resources.ErrorPackageName);
            }

            this._logger.LogInformation("Package \"{Package}\" is requested "
                + "from cache.", path);
            path = this.GetPhysicalPath(path);

            return IoFile.Exists(path)
                ? this.PhysicalFile(path, MediaTypeNames.Application.Octet)
                : this.NotFound();
        }

        /// <summary>
        /// Check whether the specified package exists.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpHead("{path}")]
        public IActionResult Head(string path) {
            if (path.ContainsInvalidFileNameChars()) {
                return this.BadRequest(Resources.ErrorPackageName);
            }

            this._logger.LogInformation("Query package \"{Package}\" from "
                + "cache.", path);
            path = this.GetPhysicalPath(path);

            return IoFile.Exists(path)
                ? this.Ok()
                : this.NotFound();
        }

        /// <summary>
        /// Adds or updates the specified package.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPut("{path}")]
        [Authorize]
        public async Task<IActionResult> Put(string path) {
            if (path.ContainsInvalidFileNameChars()) {
                return this.BadRequest(Resources.ErrorPackageName);
            }

            this._logger.LogInformation("Updating package \"{Package}\"...",
                path);
            path = this.GetPhysicalPath(path);

            using (var input = this.Request.Body)
            using (var output = IoFile.OpenWrite(path)) {
                await input.CopyToAsync(output);
            }

            this._logger.LogInformation("Package \"{Package}\" successfully "
                + "updated.", path);
            return this.NoContent();
        }

        #region Private methods
        private string GetPhysicalPath(string path)
            => Path.Combine(this._settings.Path, path);
        #endregion

        #region Private fields
        private readonly ILogger<PackageController> _logger;
        private readonly Settings _settings;
        #endregion
    }
}

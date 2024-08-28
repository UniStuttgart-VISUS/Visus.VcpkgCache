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
using System.Linq;
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
        /// Deletes the specified package.
        /// </summary>
        /// <param name="path">The path/name of the package to retrieve.</param>
        /// <returns>The contents of the package.</returns>
        [HttpDelete("{path}")]
        [Authorize]
        public IActionResult Delete(string path) {
            if (path.ContainsInvalidFileNameChars()) {
                return this.BadRequest(Resources.ErrorPackageName);
            }

            this._logger.LogDebug("Package \"{Package}\" is to be deleted from "
                + "the cache.", path);
            path = this.GetPhysicalPath(path);

            if (!IoFile.Exists(path)) {
                this._logger.LogWarning("No package exists at \"{Path}\".",
                    path);
                return this.NotFound();
            }

            IoFile.Delete(path);
            this._logger.LogInformation("Package \"{Package}\" was deleted "
                + "from the cache.", path);

            return this.NoContent();
        }

        /// <summary>
        /// Gets a list of all registered packages to authorised users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Produces("application/json")]
        public IActionResult Get() {
            this._logger.LogDebug("Package list requested.");
            var files = from f in Directory.GetFiles(this._settings.Path)
                        select Path.GetFileNameWithoutExtension(f);
            return this.Ok(files);
        }

        /// <summary>
        /// Gets the specified package.
        /// </summary>
        /// <param name="path">The path/name of the package to retrieve.</param>
        /// <returns>The contents of the package.</returns>
        [HttpGet("{path}")]
        public IActionResult Get(string path) {
            if (path.ContainsInvalidFileNameChars()) {
                return this.BadRequest(Resources.ErrorPackageName);
            }

            this._logger.LogDebug("Package \"{Package}\" is requested from "
                + "cache.", path);
            path = this.GetPhysicalPath(path);

            if (!IoFile.Exists(path)) {
                this._logger.LogWarning("No package exists at \"{Path}\".",
                    path);
                return this.NotFound();
            }

            return this.PhysicalFile(path, MediaTypeNames.Application.Octet);
        }

        /// <summary>
        /// Check whether the specified package exists.
        /// </summary>
        /// <param name="path">The path/name of the package to check.</param>
        /// <returns>HTTP 200 or 404, depending on whether the package exists.
        /// </returns>
        [HttpHead("{path}")]
        public IActionResult Head(string path) {
            if (path.ContainsInvalidFileNameChars()) {
                return this.BadRequest(Resources.ErrorPackageName);
            }

            this._logger.LogDebug("Check existence of package \"{Package}\" in "
                + "cache.", path);
            path = this.GetPhysicalPath(path);

            return IoFile.Exists(path)
                ? this.Ok()
                : this.NotFound();
        }

        /// <summary>
        /// Adds or updates the specified package.
        /// </summary>
        /// <param name="path">The path/name of the package to update.</param>
        /// <returns>The state of success of the operation.</returns>
        [HttpPut("{path}")]
        [Authorize]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Put(string path) {
            if (path.ContainsInvalidFileNameChars()) {
                return this.BadRequest(Resources.ErrorPackageName);
            }

            this._logger.LogDebug("Updating package \"{Package}\"...", path);
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
        private string GetPhysicalPath(string path) {
            var retval = Path.Combine(this._settings.Path, path);

            if (this._settings.EnforceExtension != null) {
                retval = Path.ChangeExtension(retval,
                    this._settings.EnforceExtension);
            }

            this._logger.LogTrace("Physical package path of \"{Package}\" has "
                + "been resolved to \"{Path}\".", path, retval);
            return retval;
        }
        #endregion

        #region Private fields
        private readonly ILogger<PackageController> _logger;
        private readonly Settings _settings;
        #endregion
    }
}

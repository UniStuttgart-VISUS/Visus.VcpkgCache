// <copyright file="StringExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for more information.
// </copyright>
// <author>Christoph Müller</author>

using System.IO;
using System.Linq;


namespace Visus.VcpkgCache {

    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    internal static class StringExtensions {

        /// <summary>
        /// Answer whether <paramref name="that"/> designates a valid file name.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static bool ContainsInvalidFileNameChars(this string that) {
            var invalid = Path.GetInvalidFileNameChars();
            return (that?.Any(c => invalid.Contains(c)) == true);
        }

    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludedPath.cs" company="Microsoft Corporation">
//   Copyright (c) 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
// </copyright>
// <summary>
//   A path to exclude from protection via configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System.Configuration;

    /// <summary>
    /// A path to exclude from protection via configuration.
    /// </summary>
    public sealed class ExcludedPath : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludedPath"/> class.
        /// </summary>
        public ExcludedPath()
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludedPath"/> class.
        /// </summary>
        /// <param name="path">The path to exclude.</param>
        public ExcludedPath(string path)
        {
            this.Path = path;
        }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path to exclude from protection.</value>
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get
            {
                return this["path"] as string;
            }

            set
            {
                this["path"] = value;
            }
        }
    }
}

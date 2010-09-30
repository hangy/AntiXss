// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseSecurityRuntimeConfiguration.cs" company="Microsoft Corporation">
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
//   A base class for all SRE plug-in configuration elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System.Configuration;

    /// <summary>
    /// A base class for all SRE plug-in configuration elements.
    /// </summary>
    public abstract class BaseSecurityRuntimeConfiguration : ConfigurationSection
    {
        /// <summary>
        /// The configuration property name for the excluded paths collection.
        /// </summary>
        protected const string DefaultExcludedPathsProperty = "excludedPaths";

        /// <summary>
        /// Gets the excluded paths for the plug-in.
        /// </summary>
        /// <value>The excluded paths for the plug-in.</value>
        public abstract ExcludedPathCollection ExcludedPaths
        {
            get;
        }
    }
}

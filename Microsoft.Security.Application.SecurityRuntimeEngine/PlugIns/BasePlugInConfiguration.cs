// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasePlugInConfiguration.cs" company="Microsoft Corporation">
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
//   The basic configuration elements needed for an SRE plug-in.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System.Configuration;

    /// <summary>
    /// The basic configuration elements needed for an SRE plug-in.
    /// </summary>
    public abstract class BasePlugInConfiguration : BaseSecurityRuntimeConfiguration
    {
        /// <summary>
        /// Gets the excluded paths for the plug-in.
        /// </summary>
        /// <value>The excluded paths for the plug-in.</value>
        [ConfigurationProperty(DefaultExcludedPathsProperty, IsDefaultCollection = true, IsRequired = false)]
        public override ExcludedPathCollection ExcludedPaths
        {
            get
            {
                return this[DefaultExcludedPathsProperty] as ExcludedPathCollection;
            }
        }
    }
}

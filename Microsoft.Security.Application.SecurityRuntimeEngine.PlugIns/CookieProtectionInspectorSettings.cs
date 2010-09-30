// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieProtectionInspectorSettings.cs" company="Microsoft Corporation">
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
//   Settings for the Cookie Protection Inspector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System.Configuration;

    /// <summary>
    /// Settings for the Cookie Protection Inspector.
    /// </summary>
    public sealed class CookieProtectionInspectorSettings : BasePlugInConfiguration
    {
        /// <summary>
        /// The property name for the ExcludedCookies collection.
        /// </summary>
        private const string ExcludedCookieProperty = "excludedCookies";

        /// <summary>
        /// Gets the excluded cookies for the plug-in.
        /// </summary>
        /// <value>The excluded cookies for the plug-in.</value>
        [ConfigurationProperty(ExcludedCookieProperty, IsDefaultCollection = true, IsRequired = false)]
        public NameConfigurationElementCollection ExcludedCookies
        {
            get
            {
                return this[ExcludedCookieProperty] as NameConfigurationElementCollection;
            }
        }
    }
}


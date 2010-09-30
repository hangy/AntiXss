// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurablePlugIn.cs" company="Microsoft Corporation">
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
//   Defines methods and properties that must be implemented for plug-in configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    /// <summary>
    /// Defines methods and properties that must be implemented for plug-in configuration.
    /// </summary>
    public interface IConfigurablePlugIn
    {
        /// <summary>
        /// Gets the configuration section name for the plug-in.
        /// </summary>
        /// <value>The configuration section name for the plug-in.</value>
        string ConfigurationSectionName
        {
            get;
        }

        /// <summary>
        /// Gets or sets the settings for the plug-in.
        /// </summary>
        /// <value>The settings for the plug-in.</value>
        BasePlugInConfiguration Settings
        {
            get;
            set;
        }
    }
}

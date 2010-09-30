// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisabledPlugIn.cs" company="Microsoft Corporation">
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
//   Represents a disabled plug-in entry in a configuration file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System.Configuration;

    /// <summary>
    /// Represents a disabled plug-in entry in a configuration file.
    /// </summary>
    internal sealed class DisabledPlugIn : ConfigurationElement
    {
        /// <summary>
        /// The configuration property name for the excluded plug-in configuration key.
        /// </summary>
        private const string IndexProperty = "name";

        /// <summary>
        /// The configuration property name for the excluded plug-in type.
        /// </summary>
        private const string TypeProperty = "type";

        /// <summary>
        /// Gets or sets the name of the plug-in entry.
        /// </summary>
        /// <value>
        /// The name of the plug-in entry.
        /// </value>
        [ConfigurationProperty(IndexProperty, IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return this[IndexProperty] as string;
            }

            set
            {
                this[IndexProperty] = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the disabled plug-in.
        /// </summary>
        /// <value>The type of the disabled plug-in.</value>
        [ConfigurationProperty(TypeProperty, IsRequired = true)]
        public string PlugInType
        {
            get
            {
                return this[TypeProperty] as string;
            }

            set
            {
                this[TypeProperty] = value;
            }
        }
    }
}

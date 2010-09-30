// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameConfigurationElement.cs" company="Microsoft Corporation">
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
//   A generic configuration element, "name" for use in collections.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System.Configuration;

    /// <summary>
    /// A generic configuration element, "name" for use in collections.
    /// </summary>
    public class NameConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// The property name for the name attribute
        /// </summary>
        private const string NameValueProperty = "name";

        /// <summary>
        /// Initializes a new instance of the <see cref="NameConfigurationElement"/> class.
        /// </summary>
        public NameConfigurationElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameConfigurationElement"/> class.
        /// </summary>
        /// <param name="newName">The new name.</param>
        public NameConfigurationElement(string newName)
        {
            this.Name = newName;
        }

        /// <summary>
        /// Gets the name value for this configuration element.
        /// </summary>
        /// <value>The name value for this configuration element.</value>
        [ConfigurationProperty(NameValueProperty, IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return this[NameValueProperty] as string;
            }

            private set
            {
                this[NameValueProperty] = value;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}

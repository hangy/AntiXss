// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlInjectionInspectorSettings.cs" company="Microsoft Corporation">
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
//   Settings for the SQL Injection Inspector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System.Configuration;

    /// <summary>
    /// Settings for the SQL Injection Inspector.
    /// </summary>
    public sealed class SqlInjectionInspectorSettings : BasePlugInConfiguration
    {
        /// <summary>
        /// The property name for the configuration attribute for header inspection configuration.
        /// </summary>
        private const string InspectHeadersProperty = "inspectHeaders";

        /// <summary>
        /// The property name for the configuration attribute for cookie inspection configuration.
        /// </summary>
        private const string InspectCookiesProperty = "inspectCookies";

        /// <summary>
        /// The property name for the ignored form parameters collection.
        /// </summary>
        private const string IgnoredFormParametersProperty = "ignoredFormParameters";

        /// <summary>
        /// The property name for the ignored form parameters collection.
        /// </summary>
        private const string IgnoredCookiesProperty = "ignoredCookies";

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlInjectionInspectorSettings"/> class.
        /// </summary>
        public SqlInjectionInspectorSettings()
        {
            this.IgnoredFormParameterNames.Add(new NameConfigurationElement("__VIEWSTATE"));
            this.IgnoredFormParameterNames.Add(new NameConfigurationElement("__EVENTTARGET"));
            this.IgnoredFormParameterNames.Add(new NameConfigurationElement("__REQUESTDIGEST"));
        }

        /// <summary>
        /// Gets or sets a value indicating whether Http headers should be inspected.
        /// </summary>
        /// <value>A value indicating if Http headers should be inspected.</value>
        [ConfigurationProperty(InspectHeadersProperty, IsRequired = false, DefaultValue = true)]
        public bool InspectHeaders
        {
            get
            {
                return (bool)this[InspectHeadersProperty];
            }

            set
            {
                this[InspectHeadersProperty] = value;                
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cookies should be inspected.
        /// </summary>
        /// <value>A value indicating if cookies should be inspected.</value>
        [ConfigurationProperty(InspectCookiesProperty, IsRequired = false, DefaultValue = true)]
        public bool InspectCookies
        {
            get
            {
                return (bool)this[InspectCookiesProperty];
            }

            set
            {
                this[InspectCookiesProperty] = value;
            }
        }

        /// <summary>
        /// Gets the parameter names to ignore when processing a form.
        /// </summary>
        /// <value>The excluded form parameters for the plug-in.</value>
        [ConfigurationProperty(IgnoredFormParametersProperty, IsDefaultCollection = false, IsRequired = false)]
        public NameConfigurationElementCollection IgnoredFormParameterNames
        {
            get
            {
                return this[IgnoredFormParametersProperty] as NameConfigurationElementCollection;
            }
        }

        /// <summary>
        /// Gets the cookie names to ignore when processing the cookie collection.
        /// </summary>
        /// <value>The excluded cookies for the plug-in.</value>
        [ConfigurationProperty(IgnoredCookiesProperty, IsDefaultCollection = false, IsRequired = false)]
        public NameConfigurationElementCollection IgnoredCookieNames
        {
            get
            {
                return this[IgnoredCookiesProperty] as NameConfigurationElementCollection;
            }
        }
    }
}

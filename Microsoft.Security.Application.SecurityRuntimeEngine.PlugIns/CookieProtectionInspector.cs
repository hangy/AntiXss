// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieProtectionInspector.cs" company="Microsoft Corporation">
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
//   Implements a page inspector which sets the cookie property HttpOnly to true.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System;
    using System.ComponentModel.Composition;
    using System.Web;
    using System.Web.UI;

    /// <summary>
    /// A page inspector which sets the cookie property HttpOnly to true.
    /// </summary>
    [Export(typeof(IPageInspector))]
    public class CookieProtectionInspector : IPageInspector, IConfigurablePlugIn
    {
        /// <summary>
        /// The section name for the setting section in the configuration file.
        /// </summary>
        private const string ConfigSectionName = "sreCookieSettings";
        
        /// <summary>
        /// Internal, strongly typed settings.
        /// </summary>
        private CookieProtectionInspectorSettings internalSettings = new CookieProtectionInspectorSettings();

        /// <summary>
        /// Gets the list of excluded paths for this
        /// </summary>
        /// <value></value>
        public ExcludedPathCollection ExcludedPaths
        {
            get
            {
                return this.internalSettings == null ? null : this.internalSettings.ExcludedPaths;
            }
        }

        /// <summary>
        /// Gets the configuration section name for the module.
        /// </summary>
        /// <value>The configuration section name for the module.</value>
        public string ConfigurationSectionName
        {
            get
            {
                return ConfigSectionName;
            }
        }

        /// <summary>
        /// Gets or sets the settings for the plug-in.
        /// </summary>
        /// <value>The settings for the plug-in.</value>
        public BasePlugInConfiguration Settings
        {
            get
            {
                return this.internalSettings;
            }

            set
            {
                this.internalSettings = (CookieProtectionInspectorSettings)value;
            }
        }

        /// <summary>
        /// Inspects the cookies and sets the HttpOnly attribute to true.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <param name="excludedCookiesConfig">The excluded cookies configuration.</param>
        public static void Inspect(HttpCookie cookie, CookieProtectionInspectorSettings excludedCookiesConfig)
        {
            if (cookie == null)
            {
                throw new ArgumentNullException("cookie");
            }

            if (excludedCookiesConfig != null)
            {
                // If there are excluded cookies and this cookie is excluded from protection.
                if (excludedCookiesConfig.ExcludedCookies != null)
                {
                    if (excludedCookiesConfig.ExcludedCookies.IndexOf(cookie.Name) > -1)
                    {
                        return;
                    }
                }
            }

            cookie.HttpOnly = true;
        }

        /// <summary>
        /// Inspects all HTTP page cookies and sets their HttpOnly attribute to true.
        /// </summary>
        /// <param name="page">The <see cref="Page"/> to inspect.</param>
        /// <returns>
        /// An <see cref="IInspectionResult"/> containing the results of the inspection.
        /// </returns>
        public IInspectionResult Inspect(Page page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            for (int i = 0; i < page.Response.Cookies.Count; ++i)
            {
                Inspect(page.Response.Cookies[i], this.internalSettings);
            }

            return new PageInspectionResult(InspectionResultSeverity.Continue);
        }
    }
}

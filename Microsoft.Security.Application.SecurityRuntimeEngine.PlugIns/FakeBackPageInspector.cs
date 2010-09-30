// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeBackPageInspector.cs" company="Microsoft Corporation">
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
//   A page inspector to look for fake postback requests where viewstate and other reserved field names
//   appear outside of the Request.Forms collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System;
    using System.ComponentModel.Composition;
    using System.Web.UI;

    /// <summary>
    /// A page inspector to look for fake postback requests where viewstate and other reserved field names appear outside of the Request.Forms collection.
    /// </summary>
    [Export(typeof(IPageInspector))]
    public sealed class FakeBackPageInspector : IPageInspector, IConfigurablePlugIn
    {
        /// <summary>
        /// The configuration section name,
        /// </summary>
        private const string ConfigSectionName = "sreFakeBackSettings";

        /// <summary>
        /// Internal, strongly typed settings.
        /// </summary>        
        private FakeBackInspectorSettings internalSettings = new FakeBackInspectorSettings();

        /// <summary>
        /// Gets the list of excluded paths for the plug-in.
        /// </summary>
        /// <value>The list of excluded paths for the plug-in.</value>
        public ExcludedPathCollection ExcludedPaths
        {
            get
            {
                return this.internalSettings == null ? null : this.internalSettings.ExcludedPaths;
            }
        }

        /// <summary>
        /// Gets the configuration section name for the plug-in.
        /// </summary>
        /// <value>The configuration section name for the plug-in.</value>
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
                this.internalSettings = (FakeBackInspectorSettings)value;
            }
        }

        /// <summary>
        /// Checks if a postback has been detected, but it is not from an HTTP POST.
        /// </summary>
        /// <param name="isPostBack">A value indicating whether ASP.NET considers this a postback.</param>
        /// <param name="httpMethod">The HTTP met</param>
        /// <returns>
        /// An <see cref="IInspectionResult"/> containing the results of the inspection.
        /// </returns>
        public static IInspectionResult Inspect(bool isPostBack, string httpMethod)
        {
            if (isPostBack && httpMethod != "POST")
            {
                return new PageInspectionResult(InspectionResultSeverity.Halt, null);
            }

            return new PageInspectionResult(InspectionResultSeverity.Continue);
        }

        /// <summary>
        /// Inspects an HTTP page for potential problems.
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
            
            return Inspect(page.IsPostBack, page.Request.HttpMethod);
        }
    }
}

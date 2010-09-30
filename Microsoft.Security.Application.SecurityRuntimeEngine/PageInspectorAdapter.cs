// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageInspectorAdapter.cs" company="Microsoft Corporation">
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
//
// </copyright>
// <summary>
//   Adapts a page inspector to use the common IInspector interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System.Web;
    using System.Web.UI;

    using PlugIns;

    /// <summary>
    /// Adapts a page inspector to use the common IInspector interface.
    /// </summary>
    internal sealed class PageInspectorAdapter : IInspector, IPageInspector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageInspectorAdapter"/> class.
        /// </summary>
        /// <param name="pageInspector">The page inspector.</param>
        public PageInspectorAdapter(IPageInspector pageInspector)
        {
            this.PageInspector = pageInspector;
        }

        /// <summary>
        /// Gets the list of excluded paths for the plug-in.
        /// </summary>
        /// <value>The list of excluded paths for the plug-in.</value>
        public ExcludedPathCollection ExcludedPaths
        {
            get
            {
                return this.PageInspector.ExcludedPaths;
            }
        }

        /// <summary>
        /// Gets or sets the page inspector.
        /// </summary>
        /// <value>The page inspector.</value>
        private IPageInspector PageInspector
        {
            get;
            set;
        }

        /// <summary>
        /// Inspects an HTTP page for potential problems.
        /// </summary>
        /// <param name="page">The <see cref="Page"/> to inspect.</param>
        /// <returns>An <see cref="IInspectionResult"/> containing the results of the inspection.</returns>
        public IInspectionResult Inspect(Page page)
        {
            return this.PageInspector.Inspect(page);
        }

        /// <summary>
        /// Inspects the specified request, response or page.
        /// </summary>
        /// <param name="request">The request to inspect.</param>
        /// <param name="response">The response to inspect.</param>
        /// <param name="page">The page to inspect.</param>
        /// <returns>An <see cref="IInspectionResult"/> containing the results of the inspection.</returns>
        public IInspectionResult Inspect(HttpRequestBase request, HttpResponseBase response, Page page)
        {
            return this.Inspect(page);
        }
    }
}

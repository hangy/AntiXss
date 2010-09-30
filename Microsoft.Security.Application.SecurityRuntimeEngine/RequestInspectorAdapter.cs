// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestInspectorAdapter.cs" company="Microsoft Corporation">
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
//   Adapts a request inspector to use the common IInspector interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System.Web;

    using PlugIns;

    /// <summary>
    /// Adapts a request inspector to use the common IInspector interface.
    /// </summary>
    internal sealed class RequestInspectorAdapter : IInspector, IRequestInspector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestInspectorAdapter"/> class.
        /// </summary>
        /// <param name="requestInspector">The request inspector.</param>
        public RequestInspectorAdapter(IRequestInspector requestInspector)
        {
            this.RequestInspector = requestInspector;
        }

        /// <summary>
        /// Gets the list of excluded paths for the plug-in.
        /// </summary>
        /// <value>The list of excluded paths for the plug-in.</value>
        public ExcludedPathCollection ExcludedPaths
        {
            get
            {
                return this.RequestInspector.ExcludedPaths;
            }
        }

        /// <summary>
        /// Gets or sets the request inspector.
        /// </summary>
        /// <value>The request inspector.</value>
        private IRequestInspector RequestInspector
        {
            get;
            set;
        }

        /// <summary>
        /// Inspects the specified request, response or page.
        /// </summary>
        /// <param name="request">The request to inspect.</param>
        /// <param name="response">The response to inspect.</param>
        /// <param name="page">The page to inspect.</param>
        /// <returns>
        /// An <see cref="IInspectionResult"/> containing the results of the inspection.
        /// </returns>
        public IInspectionResult Inspect(HttpRequestBase request, HttpResponseBase response, System.Web.UI.Page page)
        {
            return this.Inspect(request);
        }

        /// <summary>
        /// Inspects an HTTP request for potential problems.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestBase"/> to inspect.</param>
        /// <returns>
        /// An <see cref="IInspectionResult"/> containing the results of the inspection.
        /// </returns>
        public IInspectionResult Inspect(HttpRequestBase request)
        {
            return this.RequestInspector.Inspect(request);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseHeaderInspectorAdapter.cs" company="Microsoft Corporation">
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
//   Adapts a response header inspector to use the common IInspector interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine
{
    using System.Web;

    using PlugIns;

    /// <summary>
    /// Adapts a response header inspector to use the common <see cref="IInspector" /> interface.
    /// </summary>
    internal sealed class ResponseHeaderInspectorAdapter : IInspector, IResponseHeaderInspector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseHeaderInspectorAdapter"/> class.
        /// </summary>
        /// <param name="responseHeaderInspector">The response header inspector.</param>
        public ResponseHeaderInspectorAdapter(IResponseHeaderInspector responseHeaderInspector)
        {
            this.ResponseHeaderInspector = responseHeaderInspector;
        }

        /// <summary>
        /// Gets the list of excluded paths for the plug-in.
        /// </summary>
        /// <value>The list of excluded paths for the plug-in.</value>
        public ExcludedPathCollection ExcludedPaths
        {
            get
            {
                return this.ResponseHeaderInspector.ExcludedPaths;
            }
        }

        /// <summary>
        /// Gets or sets the response header inspector.
        /// </summary>
        /// <value>The response header inspector.</value>
        private IResponseHeaderInspector ResponseHeaderInspector
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
            return this.Inspect(request, response);
        }

        /// <summary>
        /// Inspects an HTTP response's headers for potential problems.
        /// </summary>
        /// <param name="request">The original <see cref="HttpRequestBase"/> for this response.</param>
        /// <param name="response">A <see cref="HttpResponseBase"/> to inspect.</param>
        /// <returns>
        /// An <see cref="IInspectionResult"/> containing the results of the inspection.
        /// </returns>
        /// <remarks>
        /// <para>This method is called at the point in the ASP.NET pipeline where you may still edit response headers
        /// via <see cref="HttpResponse.AppendHeader"/>.</para>
        /// <para>Accessing the <see cref="HttpResponse.Headers"/> property is only supported with the IIS 7.0 integrated pipeline mode
        /// and at least the .NET Framework 3.0. When you try to access the Headers property and either of these two conditions
        /// is not met, a PlatformNotSupportedException is thrown.</para>
        /// </remarks>
        public IInspectionResult Inspect(HttpRequestBase request, HttpResponseBase response)
        {
            return this.ResponseHeaderInspector.Inspect(request, response);
        }
    }
}

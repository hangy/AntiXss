// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResponseHeaderInspector.cs" company="Microsoft Corporation">
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
//   Defines methods that must be implemented for response header inspection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.SecurityRuntimeEngine.PlugIns
{
    using System.Web;

    /// <summary>
    /// Defines methods that must be implemented for response header inspection.
    /// </summary>
    public interface IResponseHeaderInspector : ISecurityRuntimePlugIn
    {
        /// <summary>
        /// Inspects an HTTP response's headers for potential problems.
        /// </summary>
        /// <param name="request">The original <see cref="HttpRequestBase"/> for this response.</param>
        /// <param name="response">A <see cref="HttpResponseBase"/> to inspect.</param>
        /// <returns>An <see cref="IInspectionResult"/> containing the results of the inspection.</returns>
        /// <remarks>
        /// <para>This method is called at the point in the ASP.NET pipeline where you may still edit response headers
        /// via <see cref="HttpResponse.AppendHeader"/>.</para>
        /// <para>Accessing the <see cref="HttpResponse.Headers"/> property is only supported with the IIS 7.0 integrated pipeline mode 
        /// and at least the .NET Framework 3.0. When you try to access the Headers property and either of these two conditions 
        /// is not met, a PlatformNotSupportedException is thrown.</para>
        /// </remarks>
        IInspectionResult Inspect(HttpRequestBase request, HttpResponseBase response);
    }
}
